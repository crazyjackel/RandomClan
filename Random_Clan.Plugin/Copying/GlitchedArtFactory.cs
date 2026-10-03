using HarmonyLib;
using Random_Clan.Plugin.Constants;
using Random_Clan.Plugin.Extensions;
using TrainworksReloaded.Base.Extensions;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

namespace Random_Clan.Plugin.Copying
{
    /// <summary>
    /// Clones donor unit art prefabs and applies built-in CardEffects Distortion / character tint
    /// so randomized monsters read as visually wrong without mutating shared vanilla assets.
    /// </summary>
    public sealed class GlitchedArtFactory
    {
        private readonly Dictionary<string, List<GameObject>> _clonesBySlot = new();
        private Material? _cardEffectsTemplate;

        public void Apply(CardData slot, CardData donor, CharacterData slotCharacter, CharacterData donorCharacter)
        {
            DestroyPreviousClones(slot.name);

            var cardOk = TryApplyCardArt(slot, donor);
            var characterOk = TryApplyCharacterArt(slot, slotCharacter, donorCharacter);

            if (!cardOk)
            {
                Plugin.Logger.LogWarning($"Glitch card art fallback (shared donor) for {slot.name}");
                slot.CopyCardArtFrom(donor);
            }

            if (!characterOk)
            {
                Plugin.Logger.LogWarning($"Glitch character art fallback (shared donor) for {slot.name}");
                CopyCharacterArtRef(slotCharacter, donorCharacter);
            }
        }

        private bool TryApplyCardArt(CardData slot, CardData donor)
        {
            var source = ResolveCardArt(donor);
            if (source == null)
                return false;

            var clone = InstantiateClone(source, $"{slot.name}_GlitchedCardArt");
            if (clone == null)
                return false;

            if (!ApplyCardGlitch(clone))
            {
                UnityEngine.Object.Destroy(clone);
                return false;
            }

            TrackClone(slot.name, clone);
            SetCardArtRef(slot, Wrap(clone, $"{slot.name}_GlitchedCardArt"));
            return true;
        }

        private bool TryApplyCharacterArt(CardData slot, CharacterData slotCharacter, CharacterData donorCharacter)
        {
            var source = ResolveCharacterArt(donorCharacter);
            if (source == null)
                return false;

            var clone = InstantiateClone(source, $"{slot.name}_GlitchedCharacterArt");
            if (clone == null)
                return false;

            if (!ApplyCharacterGlitch(clone))
            {
                UnityEngine.Object.Destroy(clone);
                return false;
            }

            TrackClone(slot.name, clone);
            SetCharacterArtRef(slotCharacter, Wrap(clone, $"{slot.name}_GlitchedCharacterArt"));
            return true;
        }

        private static GameObject? ResolveCardArt(CardData donor)
        {
            if (donor.GetCardArtPrefabVariant(out var prefab) && prefab != null)
                return prefab;

            var field = AccessTools.Field(typeof(CardData), "cardArtPrefabVariantRef");
            return ResolveAssetReference(field?.GetValue(donor) as AssetReferenceGameObject);
        }

        private static GameObject? ResolveCharacterArt(CharacterData donor)
        {
            try
            {
                var prefab = donor.GetCharacterPrefabVariant();
                if (prefab != null)
                    return prefab;
            }
            catch (Exception ex)
            {
                Plugin.Logger.LogWarning($"GetCharacterPrefabVariant failed: {ex.Message}");
            }

            var field = AccessTools.Field(typeof(CharacterData), "characterPrefabVariantRef");
            return ResolveAssetReference(field?.GetValue(donor) as AssetReferenceGameObject);
        }

        private static GameObject? ResolveAssetReference(AssetReferenceGameObject? assetRef)
        {
            if (assetRef == null || assetRef.IsNull())
                return null;

            if (assetRef.Asset is GameObject loaded)
                return loaded;

            try
            {
                var op = assetRef.LoadAsset();
                if (op != null && op.IsDone && op.Result is GameObject result)
                    return result;
            }
            catch (Exception ex)
            {
                Plugin.Logger.LogWarning($"Addressable art load failed: {ex.Message}");
            }

            return null;
        }

        private static GameObject? InstantiateClone(GameObject source, string name)
        {
            try
            {
                var clone = UnityEngine.Object.Instantiate(source);
                clone.name = name;
                clone.SetActive(false);
                UnityEngine.Object.DontDestroyOnLoad(clone);
                return clone;
            }
            catch (Exception ex)
            {
                Plugin.Logger.LogWarning($"Failed to instantiate art clone {name}: {ex.Message}");
                return null;
            }
        }

        private bool ApplyCardGlitch(GameObject cardArtRoot)
        {
            var image = cardArtRoot.transform.Find("CardSprite")?.GetComponent<Image>()
                        ?? cardArtRoot.GetComponentInChildren<Image>(true);
            if (image == null)
            {
                Plugin.Logger.LogWarning($"No CardSprite Image on {cardArtRoot.name}");
                return false;
            }

            var material = CreateCardMaterialInstance(image);
            if (material == null)
                return false;

            if (!EnableDistortionLayer(material))
            {
                Plugin.Logger.LogWarning($"Could not enable Distortion layer on {cardArtRoot.name}");
                return false;
            }

            image.material = material;
            var canvasRenderer = image.canvasRenderer;
            if (canvasRenderer != null)
            {
                canvasRenderer.materialCount = 1;
                canvasRenderer.SetMaterial(material, 0);
            }
            return true;
        }

        private Material? CreateCardMaterialInstance(Image image)
        {
            var current = image.material;
            if (current != null && current.HasProperty("_Layer1Enabled"))
                return new Material(current) { name = $"{current.name}_Glitched" };

            var shader = Shader.Find(ModificationTuning.CardEffectsShaderName);
            if (shader == null)
            {
                Plugin.Logger.LogWarning($"Shader not found: {ModificationTuning.CardEffectsShaderName}");
                return null;
            }

            var material = new Material(shader) { name = $"{image.name}_GlitchedCardEffects" };
            var template = GetCardEffectsTemplate();
            if (template != null)
                material.CopyPropertiesFromMaterial(template);

            if (image.sprite != null && image.sprite.texture != null && material.HasProperty("_Layer1Tex"))
                material.SetTexture("_Layer1Tex", image.sprite.texture);

            return material;
        }

        private Material? GetCardEffectsTemplate()
        {
            if (_cardEffectsTemplate != null)
                return _cardEffectsTemplate;

            _cardEffectsTemplate = Resources.FindObjectsOfTypeAll<Material>()
                .FirstOrDefault(m => m != null && m.name == ModificationTuning.CardEffectsTemplateMaterialName);
            return _cardEffectsTemplate;
        }

        private static bool EnableDistortionLayer(Material material)
        {
            var layer = FindFreeLayer(material);
            if (layer < 1)
                layer = ModificationTuning.CardEffectsMaxLayers;

            var baseStr = $"_Layer{layer}";
            if (!material.HasProperty($"{baseStr}Enabled") || !material.HasProperty($"{baseStr}Type"))
                return false;

            material.SetFloat($"{baseStr}Enabled", 1f);
            material.SetFloat($"{baseStr}Type", ModificationTuning.CardDistortionEffectType);
            material.SetFloat($"{baseStr}Stretch", 1f);
            material.SetFloat($"{baseStr}Additive", 1f);
            material.SetColor($"{baseStr}ColorTint", ModificationTuning.CardGlitchLayerTint);
            material.SetVector($"{baseStr}LinearSpeed", ModificationTuning.CardGlitchLinearSpeed);
            material.SetVector($"{baseStr}Scale", ModificationTuning.CardGlitchScale);

            if (material.HasProperty("_Layer1Tex") && material.HasProperty($"{baseStr}Motion"))
            {
                var tex = material.GetTexture("_Layer1Tex");
                if (tex != null)
                    material.SetTexture($"{baseStr}Motion", tex);
            }

            return true;
        }

        private static int FindFreeLayer(Material material)
        {
            for (var i = 1; i <= ModificationTuning.CardEffectsMaxLayers; i++)
            {
                var prop = $"_Layer{i}Enabled";
                if (!material.HasProperty(prop))
                    continue;
                if (material.GetFloat(prop) <= 0f)
                    return i;
            }
            return -1;
        }

        private static bool ApplyCharacterGlitch(GameObject characterRoot)
        {
            var applied = false;

            var quad = characterRoot.transform.Find("CharacterScale/CharacterUI/Quad_Default");
            if (quad != null)
            {
                var meshRenderer = quad.GetComponent<MeshRenderer>();
                if (meshRenderer != null)
                    applied |= TintRendererMaterials(meshRenderer);
            }

            var spine = characterRoot.transform.Find("CharacterScale/CharacterUI/SpineMeshes");
            if (spine != null && spine.gameObject.activeSelf)
            {
                foreach (var renderer in spine.GetComponentsInChildren<Renderer>(true))
                    applied |= TintRendererMaterials(renderer);
            }

            if (!applied)
            {
                foreach (var renderer in characterRoot.GetComponentsInChildren<Renderer>(true))
                    applied |= TintRendererMaterials(renderer);
            }

            return applied;
        }

        private static bool TintRendererMaterials(Renderer renderer)
        {
            if (renderer == null)
                return false;

            var materials = renderer.materials;
            if (materials == null || materials.Length == 0)
                return false;

            var any = false;
            for (var i = 0; i < materials.Length; i++)
            {
                var material = materials[i];
                if (material == null)
                    continue;

                if (material.HasProperty("_Tint"))
                {
                    material.SetColor("_Tint", ModificationTuning.CharacterGlitchTint);
                    any = true;
                }

                if (material.HasProperty("_Color"))
                {
                    var color = material.GetColor("_Color");
                    material.SetColor("_Color", Color.Lerp(color, ModificationTuning.CharacterGlitchTint, 0.45f));
                    any = true;
                }

                if (material.HasProperty("_GrayScale_Factor"))
                {
                    material.SetFloat("_GrayScale_Factor", ModificationTuning.CharacterGlitchGrayscale);
                    any = true;
                }
            }

            if (any)
                renderer.materials = materials;
            return any;
        }

        private static AssetReferenceGameObject Wrap(GameObject clone, string key)
        {
            var assetRef = new AssetReferenceGameObject();
            assetRef.SetAssetAndId(Hash128.Compute(key).ToString(), clone);
            return assetRef;
        }

        private static void SetCardArtRef(CardData slot, AssetReferenceGameObject assetRef)
            => AccessTools.Field(typeof(CardData), "cardArtPrefabVariantRef")?.SetValue(slot, assetRef);

        private static void SetCharacterArtRef(CharacterData slot, AssetReferenceGameObject assetRef)
            => AccessTools.Field(typeof(CharacterData), "characterPrefabVariantRef")?.SetValue(slot, assetRef);

        private static void CopyCharacterArtRef(CharacterData slot, CharacterData donor)
        {
            var field = AccessTools.Field(typeof(CharacterData), "characterPrefabVariantRef");
            var art = field?.GetValue(donor);
            if (art != null)
                field!.SetValue(slot, art);
        }

        private void TrackClone(string slotName, GameObject clone)
        {
            if (!_clonesBySlot.TryGetValue(slotName, out var list))
            {
                list = [];
                _clonesBySlot[slotName] = list;
            }
            list.Add(clone);
        }

        private void DestroyPreviousClones(string slotName)
        {
            if (!_clonesBySlot.TryGetValue(slotName, out var list))
                return;

            foreach (var clone in list)
            {
                if (clone != null)
                    UnityEngine.Object.Destroy(clone);
            }
            list.Clear();
        }
    }
}

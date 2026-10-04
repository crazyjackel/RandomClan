# Random Clan — Art Style Guide

Target look: **Monster Train 2** (Shiny Shoe) unit/card illustration — not the current geometric question-mark placeholders. Placeholders are temporary; replace them with MT2-fitting painted characters.

Reference vanilla MT2 clans (Banished, Pyreborne, Luna Coven, Underlegion, Lazarus League, Stygian, etc.) while painting. Match their readability, saturation, and creature personality.

## What MT2 looks like

- **Stylized fantasy cartoon**, not realism and not flat UI vector icons.
- **Crisp, saturated color** with clear local hues per clan/creature. MT2 reads brighter and sharper than MT1 — prefer punchy midtones over muddy browns.
- **Soft cel / painted shading**: simple form light, a few hard-ish highlight planes, readable darks. Avoid photoreal skin, heavy PBR, or noisy texture overlays.
- **Strong silhouettes**: units must read at card-thumbnail and combat scale. One clear pose, one clear read of “what is this creature?”
- **Personality over anatomy**: exaggerated proportions, expressive faces/props, hellish-whimsical tone (demons, beasts, fungi, constructs, cultists — weird but charming).
- **Hell-train world language**: infernal heat, scrap metal, occult glow, mycelium, pyre light, icy lunar accents — depending on the creature. Random Clan sits in that same universe; it should look native, not like a different game pasted in.
- **Busy scene, calm subject**: backgrounds can suggest depth or energy, but the creature stays the focal point. Don’t bury the silhouette in clutter.

## Random Clan flavor

Theme: **fortuitous chaos** — borrowed power, dice-luck, slapdash summoning, stitched leftovers.

Lean into:

- Mismatched gear, improvised weapons, runaway magic sparks
- Dice, coins, torn banners, glitching runes (subtle — not UI chrome)
- Teal / gold / cream clan accents when branding is needed (icons, banners), but **unit/card paintings should use creature-local colors** so they don’t all look identical

Avoid:

- Flat logo-only art as final unit/card pieces
- Generic “AI sludge” soft gradients with no silhouette
- Photoreal, anime-port, or dark-souls grit that fights MT2’s cartoon clarity
- Text-heavy UI, watermarks, or Thunderstore-style lightning marks in card/character art

## Asset types

See `README.md` in this folder for sizes and folder roles. Style notes per type:

### `card_art/`

Painted **portrait / key art** that sits inside MT2 card frames.

- Square images (examples are **256×256**; keep small for mod size).
- Subject fills most of the frame; slight crop is fine (head + torso or dramatic full body).
- Background: simple vignette, atmospheric glow, or shallow environment — not a full landscape comic panel.
- Must remain readable after the clan card border is applied (hex / spell / equipment frames crop edges).

### `character_art/`

**Combat / map character** sprite on transparent background.

- PNG with **alpha**; no opaque full-bleed backdrop.
- Full-body (or near full-body) preferred; grounded stance; facing readable for train combat.
- Outline or strong edge contrast so the unit pops on dark train rooms.
- Examples are **256×256**; can be larger if needed, but keep files lean.

### `icons/`, `banners/`, `relic_art/`, `card_borders/`

Closer to **game UI**: clearer shapes, thicker reads, less fine detail than card portraits. Clan icons can stay more graphic; still prefer MT2’s painted-icon language over pure flat geometry when replacing placeholders.

## Pipeline checklist

When replacing a placeholder `{CardId}.png`:

1. Update **both** `card_art/{CardId}.png` and `character_art/{CardId}.png` (same identity, different framing).
2. Card art = framed portrait energy; character art = transparent combat puppet.
3. Squint test at ~64px: silhouette and role still obvious.
4. Compare side-by-side with a vanilla MT2 unit of similar rarity — saturation and edge clarity should feel at home.

## Naming

Keep filenames matching JSON sprite paths (PascalCase card id), e.g. `ArbitraryHorror.png`, `ChampionA.png`. Don’t rename without updating the corresponding `json/` sprite `path`.

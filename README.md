# Fill Jars From Water (7 Days to Die V3.2)

Right-click an **empty jar** while looking at a **river or lake**. It becomes **murky water** in your inventory (same hotbar slot if you were holding a single jar). Boil it in a cooking pot.

Vanilla V3 uses `CollectWater` on the new liquid system; it often fails (especially on PGz / custom RWG water). This mod keeps CollectWater and adds a Harmony fill that uses `World.IsWater`.

Built with [Grok Build](https://grok.com).

## Requirements

- 7 Days to Die **V3.2**
- **EAC off**
- Game `Mods\0_TFP_Harmony` present

## Install

1. Close the game.
2. Download `FillJarsFromWater-1.0.1.zip` from [Releases](https://github.com/daaag0n00969/7dtd-fill-jars-from-water/releases).
3. Extract to:

```
%APPDATA%\7DaysToDie\Mods\FillJarsFromWater\
```

4. Launch without EAC.
5. Put `drinkJarEmpty` on the hotbar, look at the water surface, **hold RMB**.

Do not use LMB. Murky water (`drinkJarRiverWater`) should be boiled.

## Build from source

```
dotnet build -c Release
```

Set `GamePath` in `FillJarsFromWater.csproj`.

## Uninstall

Delete `Mods\FillJarsFromWater`.

## License

MIT

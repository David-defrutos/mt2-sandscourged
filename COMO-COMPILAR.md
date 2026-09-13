<!-- Documento generado el 2026-09-13-1651 -->

# Compilar el DLL de este mod

> El plugin lleva en `src\Plugin.cs` la **lista de ficheros JSON que carga**. Un JSON nuevo
> no existe para el juego hasta que aparece en esa lista y se recompila.
> Editar un JSON que ya esta en la lista no requiere compilar nada.

Base del codigo: tag **0.5.5** de https://github.com/AidenSheehan/MT2_Sandscourged, que es
la version del DLL que hay instalado (el `manifest.json` dice 0.5.1 y miente). Detalle en
`NOTICE.md`.

## 1. Anadir un JSON nuevo

Abre `src\Plugin.cs`, busca el bloque `c.AddMergedJsonFile(` y mete la ruta en el grupo que
toque. Las rutas son relativas a esta carpeta:

```csharp
// Rooms
"json/rooms/room_OblivionChamber.json",
"json/rooms/room_ScarabPit.json",
```

Cuidado al editarlo con un script: el fichero va en **CRLF y con BOM**. Si lo reescribes en
LF el diff sale de 266 lineas en vez de las tuyas y no hay quien lo revise.

## 2. Compilar

### 2.1. En GitHub Actions (el camino corto)

Empuja a `main` cualquier cambio dentro de `src\` y el workflow `Build DLL` arranca solo.
Tambien se puede lanzar a mano desde la pestana **Actions -> Build DLL -> Run workflow**.

Actions resuelve las credenciales de `nuget.pkg.github.com` con el `GITHUB_TOKEN` del propio
runner, y en repos publicos es gratis e ilimitado.

El resumen del run dice **cuantas rutas JSON declara tu Plugin.cs**. Si anades un fichero y
se te olvida la linea, ese numero te lo canta sin abrir nada.

### 2.2. En local

Necesitas el **SDK de .NET 9**. El `src\nuget.config` apunta a dos fuentes, y una de ellas
(`nuget.pkg.github.com`) pide credenciales de GitHub aunque el paquete sea publico. Con un
token personal con permiso `read:packages`:

```powershell
cd "C:\Users\david\AppData\Roaming\Thunderstore Mod Manager\DataFolder\MonsterTrain2\profiles\Default\BepInEx\plugins\David-Sandscourged_Custom"
dotnet nuget update source monster-train-packages -u TU_USUARIO -p TU_TOKEN --store-password-in-clear-text --configfile .\src\nuget.config
dotnet restore .\src
dotnet build .\src -c Release --output D:\Juegos\MT2_mod\_dll-build\out
```

## 3. Instalar el DLL

**La salida del build no puede quedarse dentro de `plugins\`.** BepInEx escanea esa carpeta
en profundidad buscando DLLs, y una copia en `src\bin\` haria que cargase el plugin dos
veces. Por eso el `--output` apunta fuera y luego se copia solo el DLL.

**El artefacto de Actions se descarga en ZIP**, aunque dentro vaya un solo fichero.

```powershell
$mod = "C:\Users\david\AppData\Roaming\Thunderstore Mod Manager\DataFolder\MonsterTrain2\profiles\Default\BepInEx\plugins\David-Sandscourged_Custom"
$dl  = "D:\Overnet\uTorrent\Descargas"
$bak = "D:\Juegos\MT2_mod\backups"

# copia de seguridad del DLL que funciona, SIEMPRE antes
Copy-Item "$mod\mt2_sandscourged.Plugin.dll" "$bak\dll-sandscourged-0.5.5-original.dll"

$zip = Get-ChildItem "$dl\mt2_sandscourged.Plugin*.zip" | Sort-Object LastWriteTime | Select-Object -Last 1
Expand-Archive $zip.FullName -DestinationPath "$dl\dll-nuevo" -Force
Copy-Item "$dl\dll-nuevo\mt2_sandscourged.Plugin.dll" $mod -Force

Get-Item "$mod\mt2_sandscourged.Plugin.dll" | Select-Object Length, LastWriteTime
```

El DLL actual son **49.152 bytes**. Compara.

## 4. Comprobar

```powershell
$log = "$env:USERPROFILE\AppData\LocalLow\Shiny Shoe\MonsterTrain2\Player.log"
Select-String -Path $log -Pattern "CATASTROPHIC|Exception|Failed to load" | Select-Object -First 10
& D:\Juegos\MT2_mod\scripts\validate-mt2-mods.ps1
```

Para cada carta nueva tienen que salir, por este orden: `CardUpgradeRegister`,
`SpriteRegister`, `GameObjectRegister`, `CardDataRegister` y al final `CardDataFinalizer`.
Si falta el `Finalizer`, el fichero se carga pero algo de dentro no cuadra.

## 5. Marcha atras

```powershell
Copy-Item "D:\Juegos\MT2_mod\backups\dll-sandscourged-0.5.5-original.dll" `
          "$mod\mt2_sandscourged.Plugin.dll" -Force
```

Vuelves al estado anterior. Los JSON nuevos se quedan en disco sin cargarse: no molestan.

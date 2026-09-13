<!-- Documento generado el 2026-09-13-1651 -->

# Origen y licencia

## El codigo

Todo lo que hay en `src\` es codigo del clan **The Sandscourged** de AidenSheehan, tomado
del tag `0.5.5` de https://github.com/AidenSheehan/MT2_Sandscourged.

**Ojo al numero**: el `manifest.json` de esta carpeta dice `0.5.1`, que es la version que
instalo el gestor de mods, pero el DLL que hay puesto **es el 0.5.5**: se cambio a mano en
su dia y el manifiesto no lo actualiza nadie. Comprobado leyendo las cadenas del
`mt2_sandscourged.Plugin.dll` instalado, que contienen `0.5.5` y ninguna `0.5.1`. Las 68
rutas JSON del `Plugin.cs` son identicas en los dos tags, asi que el recuento de rutas no
sirve para distinguirlos; la version del ensamblado si.

El 0.5.1 original sigue guardado en `D:\Juegos\MT2_mod\referencia\dll-sandscourged-0.5.1`.

Se distribuye bajo **licencia MIT**, y el fichero `LICENSE` de esta carpeta es el original,
con su aviso de copyright intacto:

> Copyright (c) 2025 Monster Train 2 Modding Group

**Ese aviso hay que conservarlo.** Es la unica obligacion que impone la MIT: el aviso de
copyright y el texto de la licencia tienen que viajar con el codigo y con cualquier obra
derivada. A cambio permite usarlo, modificarlo y redistribuirlo, en publico o en privado,
sin pedir permiso.

## Modificaciones propias sobre el codigo

Sobre el tag `0.5.5` hay un solo cambio, en `src\mt2_sandscourged.Plugin.csproj`: se
quitaron las reglas `<None Update="json/**">` y `<None Update="textures/**">`. En este
montaje esas dos carpetas cuelgan de la raiz del repo, no de `src\`, asi que copiarlas a la
salida del build no tenia sentido y solo despistaba.

El `src\Plugin.cs` esta **sin tocar**: sigue con las 68 rutas del tag. Cuando se anada un
JSON nuevo, la ruta se mete ahi y se recompila.

## El contenido

Los `json\` y `textures\` de la raiz son **trabajo propio derivado** del contenido original
del clan. Parten de la base **0.5.1**, que es la que instalo el gestor de mods, no de la
0.5.5: entre esas dos versiones el autor cambio 36 ficheros JSON que aqui no estan
aplicados. Se mantienen bajo MIT, que es lo coherente con la base de la que parten.

## Como anadir tu propio copyright

Si quieres constar, la forma habitual es anadir una segunda linea al `LICENSE` **sin tocar
la primera**:

```
Copyright (c) 2025 Monster Train 2 Modding Group
Copyright (c) 2026 David
```

No es obligatorio. Lo que no se puede es sustituir la linea original por la tuya.

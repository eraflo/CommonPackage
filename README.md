# Eraflo Common

Scripts et utilitaires communs partagés entre les projets FallGuys et FallGuysEditor.

## Installation

Dans Unity, ouvrir **Window > Package Manager**, cliquer sur **+** puis **Add package from disk...** et sélectionner le fichier `package.json` de ce dossier.

Ou ajouter dans `Packages/manifest.json` :
```json
{
    "dependencies": {
        "com.eraflo.common": "file:../../CommonPackage"
    }
}
```

## Structure

```
CommonPackage/
├── package.json
├── README.md
├── Runtime/
│   ├── Eraflo.Common.Runtime.asmdef
│   └── (scripts runtime)
└── Editor/
    ├── Eraflo.Common.Editor.asmdef
    └── (scripts éditeur)
```

# Frydia

Frydia est une application Windows Forms de sensibilisation à la sécurité informatique. Elle simule un faux scénario de ransomware après le déverrouillage de session Windows pour rappeler l’importance de verrouiller son poste de travail.
Frank est la pour mettre la pression à l'utilisateur, tandis que Lydia intervient pour punir et faire en sorte que l'utilisateur se souvienne de la leçon.

## Objectif

- Sensibiliser les utilisateurs à la sécurité des postes de travail.
- Montrer l’impact d’un déverrouillage non autorisé.
- Ne pas endommager les données : il s’agit d’une simulation.

## Comportement

1. Démarrage
   - L’application ouvre une instance de `Frank` sur chaque écran.
   - La session Windows est verrouillée immédiatement.

2. Phase `Frank`
   - Lors du déverrouillage, le clavier est désactivé et une alerte plein écran démarre.
   - Un compte à rebours s’affiche avec un faux message de suppression de données.
   - À la fin du compte à rebours, l’application se ferme proprement et lance `Lydia`.

3. Phase `Lydia`
   - L’utilisateur doit résoudre un calcul généré aléatoirement.
   - Le programme surveille la création de processus / fenêtres sensibles :
     - `Taskmgr` --> Le bloque et affiche un message
     - `CalculatorApp` --> Affiche un form par dessus la calculatrice avec un message
     - `excel` --> Change la formule au moment de la validation si détection
     - `SnippingTool`, `ScreenClippingHost`, `SnipAndSketch` --> Cache l'application pour évité une capture d'écran et affiche un message
   - Si Excel est détecté, la formule est renouvelée.
   - Si l’utilisateur tente de copier du contenu via `Ctrl+C`, le texte est écrasé par un autre.
   - Si l'utilisateur tente de coller du contenu via `Ctrl+V`, dans le champs, le texte est changé par un autre.
   - Si l’utilisateur saisit le code d’urgence `-8000`, l’application se ferme immédiatement.

## Installation et exécution

### Prérequis

- Windows 10/11 64 bits

### Installation

```powershell
git clone https://github.com/IAidenI/Frydia.git
cd .\Frydia
.\build.bat
```

## Architecture

- `Frydia/Frydia.csproj` : application WinForms.
- `Program.cs` : point d’entrée, gestion du multi-écran et fermeture propre.
- `Frank` : écran d’alerte et détection du déverrouillage de session.
- `Lydia` : interface de défi mathématique et surveillance des processus.
- `Hide` : interface qui permet de cacher la calculatrice
- `Calculation` : génération et validation des formules.
- `Keyboard` : permet de gérer le bloquage/débloquage du clavier
- `ProcessWatcher` / `Taskmanager` : détection et blocage des tentatives de contournement.

## TODO

- [ ] Simplifier et améliorer `TextStyle` dans `Frank`
- [ ] Refactoriser `ProcessWatcher`
- [ ] Ajouter des commentaires

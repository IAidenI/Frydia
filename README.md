# Frank

Application WinForms de sensibilisation : elle simule un faux virus après déverrouillage de session Windows.  
Ce programme est uniquement un outil de sensibilisation interne pour rappeler l’importance de verrouiller son poste de travail.

## Installation
```bash
git clone https://github.com/IAidenI/Frank.git
cd Frank && .\build.bat
.\bin\Frank.exe
```

## Fonctionnement

Au lancement, l’application ouvre une fenêtre sur chaque écran détecté puis verrouille l'ordinateur.

Elle détecte ensuite lorsque l’utilisateur déverrouille sa session Windows et affiche un message d’alerte plein écran avec un compte à rebours et bloque les interactions avec le clavier.

A la fin du compte a rebours, les fichiers ne sont pas supprimer comme pourrait le laisser penser le message mais un simple popup final s'affiche et restaure le clavier.

**Note :** Le ctrl + alt + suppr est toujours actif (car impossible à verrouillé (sécurité Windows)) et si le gestionnaire des tâches est lancé cela réactive l'affichage du alt + tab. Ce comportement peut être éviter en lançant le programme en administrateur.
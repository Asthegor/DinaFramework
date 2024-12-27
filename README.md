# DinaFramework

DinaFramework est un framework développé à partir de [MonoGame](https://github.com/MonoGame/MonoGame), conçu pour faciliter la création de jeux multiplateformes en C#.

## Fonctionnalités

- **Controls** : Composants pour gérer les entrées utilisateur (clavier et gamepad)
- **Core** : Regroupe les entités principales du framework.
- **Extensions** : Méthodes pour effectuer des conversions (ToPoint et ToVector2).
- **Functions** : Utilitaires généraux pour diverses opérations.
- **Graphics** : Outils pour la gestion et le rendu des éléments graphiques.
- **Menus** : Structures pour créer et gérer des menus dans le jeu.
- **Scenes** : Gestion des différentes scènes ou états du jeu.
- **Translation** : Support multilingue pour les jeux.



## Prérequis

- [.NET Framework 4.7.2](https://dotnet.microsoft.com/download/dotnet-framework/net472) ou version ultérieure.
- [MonoGame 3.8.2](https://github.com/MonoGame/MonoGame/releases/tag/v3.8.2) ou version ultérieure.

## Installation

1. Clonez le dépôt :

   ```bash
   git clone https://github.com/Asthegor/DinaFramework.git
   ```

2. Ouvrez le fichier `DinaFramework.sln` avec Visual Studio.
3. Restaurez les packages NuGet nécessaires.

## Utilisation

1. Ajoutez le projet `DinaFramework` à votre solution.
2. Ajoutez une référence au projet `DinaFramework` dans votre projet de jeu.
3. Dans votre classe qui hérite de `Game`, rajouter les informations suivantes :

   a. Une nouvelle variable pour contenir le gestionnaire de scène :
   ```csharp
   private SceneManager _sceneManager;
   ```
   b. Dans la fonction `Initialize`, rajoutez la ligne suivante
   pour initialiser le gestionnaire de scène :
   ```csharp
   _sceneManager = SceneManager.GetInstance(this);
   ```
   c. Dans la fonction `LoadContent`, rajoutez les lignes suivantes :
   ```csharp
   // Ajoutez toutes les scènes que vous pourrez afficher.
   // INFO : La scène ne sera chargée (Load) que lorsqu'elle sera la scène courante.
   _sceneManager.AddScene("MainMenu", typeof(MainMenu));

   // Définissez la scène de départ de votre jeu
   _sceneManager.SetCurrentScene("MainMenu");
   ```
   d. Rajoutez la ligne suivante dans la fonction `Update` :
   ```csharp
   _sceneManager.Update(gameTime);
   ```
   e. Rajoutez la ligne suivante dans la fonction `Draw` :
   ```csharp
   _sceneManager.Draw(_spriteBatch);
   ```
   Avec ces quelques lignes, chacune de vos scènes seront indépendantes les unes des autres.
   Pour plus d'informations, se référer à la documentation.

5. Vous n'avez plus qu'à utiliser les fonctionnalités du framework lors du développement de votre jeu.

## Documentation

La documentation détaillée est en cours de développement.

## Contributions

Les contributions sont les bienvenues. Veuillez soumettre des pull requests ou ouvrir des issues pour signaler des problèmes ou proposer des améliorations.

## Licence

Ce projet est sous licence MIT.

## Notes

Si vous utilisez ces sources, merci de mentionner cette page dans les crédits de votre jeu ou bien dans le fork que vous créez.

#pragma warning disable CS1591 // Commentaire XML manquant pour le type ou le membre visible publiquement
namespace DinaFramework.Enums
{
    /// <summary>
    /// Définit les alignements horizontaux possibles pour les éléments de l'interface utilisateur.
    /// </summary>
    /// <remarks>
    /// Les valeurs possibles sont :
    /// - None : Aucun alignement défini.
    /// - Left : L'élément est aligné à gauche.
    /// - Center : L'élément est centré.
    /// - Right : L'élément est aligné à droite.
    /// - Max : Valeur maximale, généralement utilisée pour la validation ou pour définir une plage d'alignement.
    /// </remarks>
    public enum HorizontalAlignment { None, Left, Center, Right, Max }
    /// <summary>
    /// Définit les alignements verticaux possibles pour les éléments de l'interface utilisateur.
    /// </summary>
    /// <remarks>
    /// Les valeurs possibles sont :
    /// - None : Aucun alignement défini.
    /// - Top : L'élément est aligné en haut.
    /// - Center : L'élément est centré verticalement.
    /// - Bottom : L'élément est aligné en bas.
    /// - Max : Valeur maximale, généralement utilisée pour la validation ou pour définir une plage d'alignement.
    /// </remarks>
    public enum VerticalAlignment { None, Top, Center, Bottom, Max }
    /// <summary>
    /// Définit les alignements possibles pour les icônes dans un menu.
    /// </summary>
    /// <remarks>
    /// Les valeurs possibles sont :
    /// - None : Aucun alignement défini.
    /// - Left : L'icône est alignée à gauche.
    /// - Right : L'icône est alignée à droite.
    /// - Both : L'icône est centrée à gauche et à droite.
    /// - Max : Valeur maximale, généralement utilisée pour la validation ou pour définir une plage d'alignement.
    /// </remarks>
    public enum IconMenuAlignment { None, Left, Right, Both, Max }
    /// <summary>
    /// Définit le sens des items du menu.
    /// </summary>
    /// <remarks>
    /// Les valeurs possibles sont :
    /// - Vertical : Les items du menu sont disposés verticalement.
    /// - Horizontal : Les items du menu sont disposés horizontalement.
    /// </remarks>
    public enum MenuItemDirection { Vertical, Horizontal }
    /// <summary>
    /// Définit les actions possibles pour un contrôleur.
    /// </summary>
    /// <remarks>
    /// Les valeurs possibles sont :
    /// - None : Aucune action n'est effectuée.
    /// - Pressed : Le bouton est pressé.
    /// - Released : Le bouton est relâché.
    /// - Continuous : Le bouton est maintenu enfoncé.
    /// - Max : Valeur maximale, généralement utilisée pour la validation ou pour définir une plage d'actions.
    /// </remarks>
    public enum ControllerAction { None, Pressed, Released, Continuous, Max }
    /// <summary>
    /// Définit les types de contrôleurs pris en charge.
    /// </summary>
    /// <remarks>
    /// Les valeurs possibles sont :
    /// - None : Aucun contrôleur spécifié.
    /// - Keyboard : Le contrôleur est un clavier.
    /// - Gamepad : Le contrôleur est une manette de jeu.
    /// - Mouse : Le contrôleur est une souris.
    /// - Max : Valeur maximale, généralement utilisée pour la validation ou pour définir une plage de types.
    /// </remarks>
    public enum ControllerType { None, Keyboard, Gamepad, Mouse, Max }
    /// <summary>
    /// Définit les types de boutons possibles pour une souris.
    /// </summary>
    /// <remarks>
    /// Les valeurs possibles sont :
    /// - None : Aucun bouton spécifié.
    /// - Left : Le bouton gauche de la souris.
    /// - Right : Le bouton droit de la souris.
    /// - Middle : Le bouton central de la souris.
    /// - ScrollWheel : La molette de défilement.
    /// - Move : Le déplacement de la souris.
    /// - Max : Valeur maximale, généralement utilisée pour la validation ou pour définir une plage de boutons.
    /// </remarks>
    public enum MouseButtonType { None, Left, Right, Middle, ScrollWheel, Move, Max }
    /// <summary>
    /// Définit les modes de progression pour une barre de progression.
    /// </summary>
    /// <remarks>
    /// Les valeurs possibles sont :
    /// - None : Aucun mode spécifié.
    /// - LeftToRight : La progression va de gauche à droite.
    /// - RightToLeft : La progression va de droite à gauche.
    /// - TopToBottom : La progression va de haut en bas.
    /// - BottomToTop : La progression va de bas en haut.
    /// - Max : Valeur maximale, généralement utilisée pour la validation ou pour définir une plage de modes.
    /// </remarks>
    public enum ProgressDirection { None, LeftToRight, RightToLeft, TopToBottom, BottomToTop, Max }
    /// <summary>
    /// Définit les états d'une case à cocher.
    /// </summary>
    /// <remarks>
    /// Les valeurs possibles sont :
    /// - Unchecked : La case à cocher est décochée.
    /// - Checked : La case à cocher est cochée.
    /// </remarks>
    public enum CheckBoxState { Unchecked, Checked };
    /// <summary>
    /// Définit les différents types de barre de progression.
    /// </summary>
    /// <remarks>
    /// Les valeurs possibles sont :
    /// - Color : La barre de progression est un simple remplissage coloré.
    /// - Image2Parts : La barre de progression utilise une image divisée en deux parties.
    /// - Image3Parts : La barre de progression utilise une image divisée en trois parties.
    /// </remarks>
    public enum ProgressBarType { Color, Image2Parts, Image3Parts }
    /// <summary>
    /// Définit les états d'un élément de menu.
    /// </summary>
    /// <remarks>
    /// Les valeurs possibles sont :
    /// - Enable : L'élément de menu est activé.
    /// - Disable : L'élément de menu est désactivé.
    /// </remarks>
    public enum MenuItemState { Enable, Disable }
}
#pragma warning restore CS1591 // Commentaire XML manquant pour le type ou le membre visible publiquement

#pragma warning disable CS1591 // Commentaire XML manquant pour le type ou le membre visible publiquement
namespace DinaFramework.Enums
{
    /// <summary>
    /// Définit les alignements horizontaux possibles pour les éléments de l'interface utilisateur.
    /// </summary>
    public enum HorizontalAlignment { None, Left, Center, Right, Max }
    /// <summary>
    /// Définit les alignements verticaux possibles pour les éléments de l'interface utilisateur.
    /// </summary>
    public enum VerticalAlignment { None, Top, Center, Bottom, Max }
    /// <summary>
    /// Définit les alignements possibles pour les icônes dans un menu.
    /// </summary>
    public enum IconMenuAlignment { None, Left, Right, Both, Max }
    /// <summary>
    /// Définit le sens des items du menu.
    /// </summary>
    public enum MenuItemDirection { Vertical, Horizontal }
    /// <summary>
    /// Définit les actions possibles pour un contrôleur.
    /// </summary>
    public enum ControllerAction { None, Pressed, Released, Continuous, Max }
    /// <summary>
    /// Définit les types de contrôleurs pris en charge.
    /// </summary>
    public enum ControllerType { None, Keyboard, Gamepad, Mouse, Max }
    /// <summary>
    /// Définit les types de boutons possibles pour une souris.
    /// </summary>
    public enum MouseButtonType { None, Left, Right, Middle, ScrollWheel, Move, Max }
    /// <summary>
    /// Définit les modes de progression pour une barre de progression.
    /// </summary>
    public enum ProgressDirection { None, LeftToRight, RightToLeft, TopToBottom, BottomToTop, Max }
    /// <summary>
    /// Définit les états d'une case à cocher.
    /// </summary>
    public enum CheckBoxState { Unchecked, Checked };
    /// <summary>
    /// Définit les différents types de barre de progression.
    /// </summary>
    public enum ProgressBarType { Color, Image2Parts, Image3Parts }
    /// <summary>
    /// Définit les états d'un élément de menu.
    /// </summary>
    public enum MenuItemState { Enable, Disable }

    public enum ResolutionFontSize { None, Small, Medium, Large, XL, XXL }
}
#pragma warning restore CS1591 // Commentaire XML manquant pour le type ou le membre visible publiquement

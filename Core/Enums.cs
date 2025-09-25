#pragma warning disable CS1591 // Pour ne pas avoir de warning sur chaque valeur des enums.
namespace DinaFramework.Enums
{
    /// <summary>
    /// Définit les alignements horizontaux possibles pour les éléments de l'interface utilisateur.
    /// Valeurs : None, Left, Center, Right, Max
    /// </summary>
    public enum HorizontalAlignment { None, Left, Center, Right, Max }

    /// <summary>
    /// Définit les alignements verticaux possibles pour les éléments de l'interface utilisateur.
    /// Valeurs : None, Top, Center, Bottom, Max
    /// </summary>
    public enum VerticalAlignment { None, Top, Center, Bottom, Max }

    /// <summary>
    /// Définit les alignements possibles pour les icônes dans un menu.
    /// Valeurs : None, Left, Right, Both, Max
    /// </summary>
    public enum IconMenuAlignment { None, Left, Right, Both, Max }

    /// <summary>
    /// Définit le sens des items du menu.
    /// Valeurs : Vertical, Horizontal
    /// </summary>
    public enum MenuItemDirection { Vertical, Horizontal }

    /// <summary>
    /// Définit les actions possibles pour un contrôleur.
    /// Valeurs : None, Pressed, Released, Continuous, Max
    /// </summary>
    public enum ControllerAction { None, Pressed, Released, Continuous, Max }

    /// <summary>
    /// Définit les types de contrôleurs pris en charge.
    /// Valeurs : None, Keyboard, Gamepad, Mouse, Max
    /// </summary>
    public enum ControllerType { None, Keyboard, Gamepad, Mouse, Max }

    /// <summary>
    /// Définit les types de boutons possibles pour une souris.
    /// Valeurs : None, Left, Right, Middle, ScrollWheel, Move, Max
    /// </summary>
    public enum MouseButtonType { None, Left, Right, Middle, ScrollWheel, Move, Max }

    /// <summary>
    /// Définit les modes de progression pour une barre de progression.
    /// Valeurs : None, LeftToRight, RightToLeft, TopToBottom, BottomToTop, Max
    /// </summary>
    public enum ProgressDirection { None, LeftToRight, RightToLeft, TopToBottom, BottomToTop, Max }

    /// <summary>
    /// Définit les états d'une case à cocher.
    /// Valeurs : Unchecked, Checked
    /// </summary>
    public enum CheckBoxState { Unchecked, Checked }

    /// <summary>
    /// Définit les différents types de barre de progression.
    /// Valeurs : Color, Image2Parts, Image3Parts
    /// </summary>
    public enum ProgressBarType { Color, Image2Parts, Image3Parts }

    /// <summary>
    /// Définit les états d'un élément de menu.
    /// Valeurs : Enable, Disable
    /// </summary>
    public enum MenuItemState { Enable, Disable }

    /// <summary>
    /// Définit les tailles de police en fonction de la résolution.
    /// Valeurs : None, Small, Medium, Large, XL, XXL
    /// </summary>
    public enum ResolutionFontSize { None, Small, Medium, Large, XL, XXL }
}
#pragma warning restore CS1591 // Pour ne pas avoir de warning sur chaque valeur des enums.
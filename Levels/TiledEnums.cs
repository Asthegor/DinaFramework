#pragma warning disable CA1720 // L'identificateur contient le nom de type
namespace DinaFramework.Levels
{
    /// <summary>
    /// Définit l’orientation de la carte Tiled.
    /// </summary>
    public enum TiledOrientation
    {
        /// <summary>
        /// Carte orthogonale (grille carrée classique).
        /// </summary>
        Orthogonal,

        /// <summary>
        /// Carte isométrique (vue en diagonale type 45°).
        /// </summary>
        Isometric,

        /// <summary>
        /// Carte en losange décalé (staggered).
        /// </summary>
        Stagged,

        /// <summary>
        /// Carte hexagonale.
        /// </summary>
        Hexagonal
    }

    /// <summary>
    /// Types de propriétés personnalisées définies dans Tiled.
    /// </summary>
    public enum TiledPropertyType
    {
        /// <summary>
        /// Valeur booléenne (true/false).
        /// </summary>
        Bool,

        /// <summary>
        /// Couleur au format hexa (#RRGGBB ou #AARRGGBB).
        /// </summary>
        Color,

        /// <summary>
        /// Référence à un fichier externe.
        /// </summary>
        File,

        /// <summary>
        /// Nombre flottant (simple précision).
        /// </summary>
        Float,

        /// <summary>
        /// Nombre entier.
        /// </summary>
        Int,

        /// <summary>
        /// Référence à un objet Tiled.
        /// </summary>
        Object,

        /// <summary>
        /// Chaîne de caractères.
        /// </summary>
        String
    }

    /// <summary>
    /// Types d’objets géométriques ou textuels présents dans un calque d’objets.
    /// </summary>
    public enum TiledObjectType
    {
        /// <summary>
        /// Objet par défaut (rectangle).
        /// </summary>
        Default,

        /// <summary>
        /// Objet de type ellipse.
        /// </summary>
        Ellipse,

        /// <summary>
        /// Objet de type point (x,y uniquement).
        /// </summary>
        Point,

        /// <summary>
        /// Objet défini par un polygone.
        /// </summary>
        Polygon,

        /// <summary>
        /// Objet textuel (zone de texte).
        /// </summary>
        Text
    }
}
#pragma warning restore CA1720 // L'identificateur contient le nom de type

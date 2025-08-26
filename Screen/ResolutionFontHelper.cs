using DinaFramework.Enums;

using Microsoft.Xna.Framework;

namespace DinaFramework.Screen
{

    /// <summary>
    /// Gestionnaire de taille de police adaptée à la résolution d'écran.
    /// Détermine la taille de police à utiliser selon la résolution 16:9 la plus grande qui tient dans l'écran.
    /// </summary>
    public static class ResolutionFontManager
    {
        private const int W720 = 1280, H720 = 720;
        private const int W1080 = 1920, H1080 = 1080;
        private const int W1440 = 2560, H1440 = 1440;
        private const int W4K = 3840, H4K = 2160;

        private const long A720 = W720 * H720;
        private const long A1080 = W1080 * H1080;
        private const long A1440 = W1440 * H1440;
        private const long A4K = W4K * H4K;

        private const double Threshold720 = A720 * 0.8;
        private const double Threshold1080 = A1080 * 0.8;
        private const double Threshold1440 = A1440 * 0.8;
        private const double Threshold4K = A4K * 0.8;

        /// <summary>
        /// Permet de récupérer le type de taille de police selon la résolution d'écran fournie.
        /// </summary>
        /// <param name="screenResolution">Résolution de l'écran.</param>
        /// <returns></returns>
        public static ResolutionFontSize GetFontSizeForResolution(Vector2 screenResolution)
        {
            return GetFontSizeForResolution((int)screenResolution.X, (int)screenResolution.Y);
        }
        /// <summary>
        /// Permet de récupérer le type de taille de police selon la résolution d'écran fournie.
        /// </summary>
        /// <param name="screenWidth">Largeur de l'écran.</param>
        /// <param name="screenHeight">Hauteur de l'écran.</param>
        /// <returns>Type de taille de police.</returns>
        public static ResolutionFontSize GetFontSizeForResolution(int screenWidth, int screenHeight)
        {
            // 1) On “ramène” la résolution à la plus grande zone 16:9 qui tient dedans
            (int w16x9, int h16x9) = FitInside16x9(screenWidth, screenHeight);

            // 2) On classe par aire, avec 1080p = Large
            double area = w16x9 * h16x9;

            return area switch
            {
                < Threshold720 => ResolutionFontSize.Small,
                < Threshold1080 => ResolutionFontSize.Medium,
                < Threshold1440 => ResolutionFontSize.Large,
                < Threshold4K => ResolutionFontSize.XL,
                _ => ResolutionFontSize.XXL
            };
        }

        /// <summary>
        /// Calcule la plus grande zone 16:9 qui rentre dans les dimensions données sans dépasser.
        /// </summary>
        /// <param name="width">Largeur disponible.</param>
        /// <param name="height">Hauteur disponible.</param>
        /// <returns>Largeur et hauteur de la zone 16:9 maximale.</returns>
        public static (int width, int height) FitInside16x9(int width, int height)
        {
            // Essai par la largeur
            int hByW = (width * 9) / 16;
            if (hByW <= height)
                return (width, hByW);

            // Sinon on s’aligne sur la hauteur
            int wByH = (height * 16) / 9;
            return (wByH, height);
        }

        /// <summary>
        /// Fournit le ratio selon la résolution (référence : ResolutionFontSize.Large)
        /// </summary>
        /// <param name="screenWidth">Largeur de l'écran.</param>
        /// <param name="screenHeight">Hauteur de l'écran.</param>
        /// <returns>Ratio entre les types de taille de police de résolution.</returns>
        public static float GetRatioFromResolution(int screenWidth, int screenHeight)
        {
            ResolutionFontSize resFontSize = GetFontSizeForResolution(screenWidth, screenHeight);
            return GetRatioForResolutionFontSize(resFontSize);
        }
        /// <summary>
        /// Fournit le ratio selon la résolution (référence : ResolutionFontSize.Large)
        /// </summary>
        /// <param name="screenDimensions">Dimensions de l'écran.</param>
        /// <returns>Ratio entre les types de taille de police de résolution.</returns>
        public static float GetRatioForResolution(Vector2 screenDimensions)
        {
            return GetRatioFromResolution((int)screenDimensions.X, (int)screenDimensions.Y);
        }
        /// <summary>
        /// Fournit le ratio selon le type de taille de police de la résolution d'écran fourni.
        /// </summary>
        /// <param name="resolutionFontSize">Te type de taille de police de la résolution d'écran.</param>
        /// <returns>Ratio entre les types de taille de police de résolution.</returns>
        public static float GetRatioForResolutionFontSize(ResolutionFontSize resolutionFontSize)
        {
            return resolutionFontSize switch
            {
                ResolutionFontSize.Small => 0.5f,
                ResolutionFontSize.Medium => 0.75f,
                ResolutionFontSize.Large => 1f,
                ResolutionFontSize.XL => 1.25f,
                ResolutionFontSize.XXL => 1.5f,
                _ => 1f // Par défaut, on renvoie toujours 1.
            };
        }
    }
}

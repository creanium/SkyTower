using System.ComponentModel;

namespace SkyTower.Core.Enums;

/*
 * Categorical outlooks contain DN attributes which are based on the NDFD
 * grid values which are defined as:
 *  2 - Thunderstorm
 *  3 - Marginal Risk
 *  4 - Slight Risk
 *  5 - Enhanced Risk
 *  6 - Moderate Risk
 *  8 - High Risk
 */
public enum CategoricalRisk
{
    None = 0,
    [Description("General Thunderstorm")]
    Thunderstorm = 2,
    Marginal = 3,
    Slight = 4,
    Enhanced = 5,
    Moderate = 6,
    High = 8
}
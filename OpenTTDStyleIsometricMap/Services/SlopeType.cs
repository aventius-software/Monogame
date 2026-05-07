namespace OpenTTDStyleIsometricMap.Services;

// Taken list originally from https://newgrf-specs.tt-wiki.net/wiki/NML:List_of_tile_slopes
// Note - the values here correspond to the tile slope type 'region' in the texture atlas, so
// if you change the order of the tiles in the atlas then you will need to update these values
// accordingly!
internal enum SlopeType : int
{
    NONE = -1,
    SLOPE_FLAT = 0,
    SLOPE_W = 4,
    SLOPE_S = 5,
    SLOPE_E = 6,
    SLOPE_N = 7,
    SLOPE_NW = 8,
    SLOPE_SW = 9,
    SLOPE_SE = 10,
    SLOPE_NE = 11,
    SLOPE_EW = 21,
    SLOPE_NS = 20,
    SLOPE_NWS = 12,
    SLOPE_WSE = 13,
    SLOPE_SEN = 14,
    SLOPE_ENW = 15,
    SLOPE_STEEP_W = 17,
    SLOPE_STEEP_S = 16,
    SLOPE_STEEP_E = 19,
    SLOPE_STEEP_N = 18
}
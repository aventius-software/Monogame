using Microsoft.Xna.Framework;
using Shared.Services;

namespace OutrunStyleTest.Track;

/// <summary>
/// A basic service to draw track segments.
/// </summary>
internal class TrackDrawingService
{
    private readonly ShapeDrawingService _shapeDrawingService;

    public TrackDrawingService(ShapeDrawingService shapeDrawingService)
    {
        _shapeDrawingService = shapeDrawingService;
    }

    public void DrawTrackSegment(
        int viewPortWidth,
        int numberOfLanes,
        int x1,
        int y1,
        int w1,
        int x2,
        int y2,
        int w2,
        Color roadColour,
        Color grassColour,
        Color rumbleColour,
        Color laneColour,
        bool drawLanes)
    {
        // Draw grass first        
        _shapeDrawingService.DrawFilledRectangle(grassColour, 0, y2, viewPortWidth, y1 - y2);

        // Draw the road surface        
        _shapeDrawingService.DrawFilledQuadrilateral(roadColour, x1 - w1, y1, x1 + w1, y1, x2 + w2, y2, x2 - w2, y2);

        // Draw rumble strips
        var rumble_w1 = w1 / 5;
        var rumble_w2 = w2 / 5;

        _shapeDrawingService.DrawFilledQuadrilateral(rumbleColour, x1 - w1 - rumble_w1, y1, x1 - w1, y1, x2 - w2, y2, x2 - w2 - rumble_w2, y2);
        _shapeDrawingService.DrawFilledQuadrilateral(rumbleColour, x1 + w1 + rumble_w1, y1, x1 + w1, y1, x2 + w2, y2, x2 + w2 + rumble_w2, y2);

        // Draw lane markers if required        
        if (drawLanes)
        {
            var line_w1 = w1 / 20 / 2;
            var line_w2 = w2 / 20 / 2;

            var lane_w1 = w1 * 2 / numberOfLanes;
            var lane_w2 = w2 * 2 / numberOfLanes;

            var lane_x1 = x1 - w1;
            var lane_x2 = x2 - w2;

            for (var i = 1; i < numberOfLanes; i++)
            {
                lane_x1 += lane_w1;
                lane_x2 += lane_w2;

                _shapeDrawingService.DrawFilledQuadrilateral(laneColour,
                    lane_x1 - line_w1, y1,
                    lane_x1 + line_w1, y1,
                    lane_x2 + line_w2, y2,
                    lane_x2 - line_w2, y2
                );
            }
        }
    }
}

// Name: Game Map Zones Tool
// Submenu:
// Author: Rhydian Carter
// Title:
// Version: 1.0
// Desc: Tool for defining zones of a game map based on a colour value.
// Keywords:
// URL:
// Help:
// Force Single Render Call

#region UICode
IntSliderControl pixelsPerChunk = 8; // [8,32] Pixels Per Chunk
IntSliderControl pixelsPerZone = 1; // [1,8] Pixels Per Zone
RadioButtonControl origin = 0; // Origin|Bottom Left|Top Left
RadioButtonControl coordMode = 1; // Coordinate Mode|Zones|Tiles
RadioButtonControl colorMethod = 0; // Colour Method|Hex Value|Wheel
ColorWheelControl wheelColor = ColorBgra.FromBgr(0, 0, 255); // [Red] {!colorMethod} Colour
TextboxControl colorText = ""; // [6] {colorMethod} Colour #
IntSliderControl chunkX = 0; // [0,98] X Coordinate - CHUNK
IntSliderControl chunkY = 0; // [0,198] Y Coordinate - CHUNK
IntSliderControl zoneX = 0; // [0,8] {coordMode} X Coordinate - ZONE
IntSliderControl zoneY = 0; // [0,8] {coordMode} Y Coordinate - ZONE
IntSliderControl tileX = 0; // [0,64] {!coordMode} X Coordinate - TILE
IntSliderControl tileY = 0; // [0,64] {!coordMode} Y Coordinate - TILE
IntSliderControl drawWidth = 1; // [1,8] Draw X Zones
IntSliderControl drawHeight = 1; // [1,8] Draw Y Zones
CheckboxControl draw = false; // Draw
#endregion

Rectangle tileRect;
ColorBgra color = ColorBgra.Red;

void PreRender(Surface dst, Surface src)
{
    if(draw == false)
    return;

    int coordXZone;
    int coordYZone;

    // Adjust zone coordinate based on mode.
    if(coordMode == 0) // Zone Mode
    {
        coordXZone = zoneX * pixelsPerZone;
        coordYZone = zoneY * pixelsPerZone;
    }
    else // Tile Mode
    {
        coordXZone = (tileX / pixelsPerChunk) * pixelsPerZone;
        coordYZone = (tileY / pixelsPerChunk) * pixelsPerZone;
    }

    // Get the rest of our inputs.
    int coordX = (chunkX * pixelsPerChunk) + coordXZone;
    int coordY = (chunkY * pixelsPerChunk) + coordYZone;
    int rectWidth = drawWidth * pixelsPerZone;
    int rectHeight = drawHeight * pixelsPerZone;

    // Set colour method based on hex or wheel.
    if(colorMethod == 0)
    color = HexToColorBgra(colorText);
    else
    color = wheelColor;

    // Don't draw if our coord is outside of the bounds.
    if(coordX >= dst.Width || coordY >= dst.Height)
    return;

    // Stop our rect from drawing larger than the canvas.
    int overflowRight = (coordX + rectWidth) - dst.Width;
    rectWidth = Math.Min(rectWidth, rectWidth - overflowRight);
    int overflowBottom = (coordY + rectHeight) - dst.Height;
    rectHeight = Math.Min(rectHeight, rectHeight - overflowBottom);

    // Flip our Y coord if we're using bottom left origin.
    if(origin == 0)
    coordY = dst.Height - coordY - rectHeight;

    // Create a rectangle to draw.
    tileRect = new Rectangle(coordX, coordY, rectWidth, rectHeight);
}


void Render(Surface dst, Surface src, Rectangle rect)
{
    if(draw == false)
    return;

    // Draw our rectangle.
    for(int x = tileRect.Left; x < tileRect.Right; x++)
    {
        for(int y = tileRect.Top; y < tileRect.Bottom; y++)
        {
            dst[x, y] = color;
        }
    }
}


// Function takes a hexadecimal colour value and turns it into a Paint.net ColorBgra.
public static ColorBgra HexToColorBgra(string hexColor)
{
    // Return default colour if our string isn't valid.
    if(hexColor.Length < 6)
    return ColorBgra.Red;

    try
    {
        int red = int.Parse(hexColor.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
        int green = int.Parse(hexColor.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
        int blue = int.Parse(hexColor.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);

        return ColorBgra.FromBgr((byte)blue, (byte)green, (byte)red);
    }
    catch(Exception)
    {
        // Return a default colour if something goes wrong.
        return ColorBgra.Red;
    }
}
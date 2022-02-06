using System;
using System.Numerics;

namespace Tofunaut.TofuECS_Rogue.Generation
{
    public class Section
    {
        public enum Corner
        {
            TopRight,
            BottomRight,
            BottomLeft,
            TopLeft,
        }
        
        public int Area => Width * Height;
        public int Width => MaxX - MinX;
        public int Height => MaxY - MinY;

        public int MinX;
        public int MinY;
        public int MaxX;
        public int MaxY;

        public void Insert(int width, int height, Corner corner, out Section insertedSection,
            out Section secondarySection)
        {
            switch(corner)
            {
                case Corner.TopRight:
                    InsertTopRight(width, height, out insertedSection, out secondarySection);
                    break;
                case Corner.BottomRight:
                    InsertBottomRight(width, height, out insertedSection, out secondarySection);
                    break;
                case Corner.BottomLeft:
                    InsertBottomLeft(width, height, out insertedSection, out secondarySection);
                    break;
                case Corner.TopLeft:
                    InsertTopLeft(width, height, out insertedSection, out secondarySection);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(corner), corner, null);
            }
        }

        public void InsertTopRight(int width, int height, out Section insertedSection, out Section secondarySection)
        {
            insertedSection = new Section
            {
                MinX = MaxX - width,
                MaxX = MaxX,
                MinY = MaxY - height,
                MaxY = MaxY,
            };

            if (width < height)
            {
                secondarySection = new Section
                {
                    MinX = MinX,
                    MaxX = MaxX - width,
                    MinY = MaxY - height,
                    MaxY = MaxY,
                };

                MaxY -= height;
            }
            else
            {
                secondarySection = new Section
                {
                    MinX = MaxX - width,
                    MaxX = MaxX,
                    MinY = MinY,
                    MaxY = MaxY - height,
                };

                MaxX -= width;
            }
        }
        
        public void InsertBottomRight(int width, int height, out Section insertedSection, out Section secondarySection)
        {
            insertedSection = new Section
            {
                MinX = MaxX - width,
                MaxX = MaxX,
                MinY = MinY,
                MaxY = MinY + height,
            };

            if (width < height)
            {
                secondarySection = new Section
                {
                    MinX = MinX,
                    MaxX = MaxX - width,
                    MinY = MinY,
                    MaxY = MinY + height,
                };

                MinY += height;
            }
            else
            {
                secondarySection = new Section
                {
                    MinX = MaxX - width,
                    MaxX = MaxX,
                    MinY = MinY + height,
                    MaxY = MaxY
                };

                MaxX -= width;
            }
        }
        
        public void InsertBottomLeft(int width, int height, out Section insertedSection, out Section secondarySection)
        {
            insertedSection = new Section
            {
                MinX = MinX,
                MaxX = MinX + width,
                MinY = MinY,
                MaxY = MinY + height,
            };

            if (width < height)
            {
                secondarySection = new Section
                {
                    MinX = MinX + width,
                    MaxX = MaxX,
                    MinY = MinY,
                    MaxY = MinY + height,
                };

                MinY += height;
            }
            else
            {
                secondarySection = new Section
                {
                    MinX = MinX,
                    MaxX = MinX + width,
                    MinY = MinY + height,
                    MaxY = MaxY
                };

                MinX += width;
            }
        }
        
        public void InsertTopLeft(int width, int height, out Section insertedSection, out Section secondarySection)
        {
            insertedSection = new Section
            {
                MinX = MinX,
                MaxX = MinX + width,
                MinY = MaxY - height,
                MaxY = MaxY,
            };

            if (width < height)
            {
                secondarySection = new Section
                {
                    MinX = MinX + width,
                    MaxX = MaxX,
                    MinY = MaxY - height,
                    MaxY = MaxY,
                };

                MaxY -= height;
            }
            else
            {
                secondarySection = new Section
                {
                    MinX = MinX,
                    MaxX = MinX + width,
                    MinY = MinY,
                    MaxY = MaxY - height,
                };

                MinX += width;
            }
        }

        public void SplitX(int x, out Section newSection)
        {
            newSection = new Section
            {
                MinX = x,
                MaxX = MaxX,
                MinY = MinY,
                MaxY = MaxY,
            };
            MaxX = x;
        }
        
        public void SplitY(int y, out Section newSection)
        {
            newSection = new Section
            {
                MinX = MinX,
                MaxX = MaxX,
                MinY = y,
                MaxY = MaxY,
            };
            MaxY = y;
        }
    }
}
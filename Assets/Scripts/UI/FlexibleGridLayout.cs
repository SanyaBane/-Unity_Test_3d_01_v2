using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class FlexibleGridLayout : LayoutGroup
    {
        public enum FitType
        {
            Uniform,
            Width,
            Height
        }

        public FitType fitType;

        public int rows;
        public int columns;
        public Vector2 cellSize;
        public Vector2 spacing;


        public override void CalculateLayoutInputHorizontal()
        {
            base.CalculateLayoutInputHorizontal();

            // float sqrRt = Mathf.Sqrt(rectChildren.Count);
            // rows = Mathf.CeilToInt(sqrRt);
            // columns = Mathf.CeilToInt(sqrRt);

            // if (fitType == FitType.Width)
            // {
            //     rows = Mathf.CeilToInt(rectChildren.Count / (float) columns);
            // }
            //
            // if (fitType == FitType.Height)
            // {
            //     rows = Mathf.CeilToInt(rectChildren.Count / (float) rows);
            // }

            float parentWidth = rectTransform.rect.width;
            float parentHeight = rectTransform.rect.height;

            float cellWidth;
            if (columns == 0)
                columns = 1;
            // if (columns == 0)
            //     cellWidth = 0;
            // else
            //     cellWidth = (parentWidth / (float) columns) - ((spacing.x / ((float) columns)) * (columns - 1)) - (padding.left / (float) columns) - (padding.right / (float) columns);
            cellWidth = (parentWidth / (float) columns) - ((spacing.x / ((float) columns)) * (columns - 1)) - (padding.left / (float) columns) - (padding.right / (float) columns);

            float cellHeight;
            if (rows == 0)
                rows = 1;
            // if (rows == 0)
            //     cellHeight = 0;
            // else
            //     cellHeight = (parentHeight / (float) rows) - ((spacing.y / ((float) rows)) * (rows - 1)) - (padding.top / (float) rows) - (padding.bottom / (float) rows);
            cellHeight = (parentHeight / (float) rows) - ((spacing.y / ((float) rows)) * (rows - 1)) - (padding.top / (float) rows) - (padding.bottom / (float) rows);

            cellSize.x = cellWidth;
            cellSize.y = cellHeight;

            int columnCount = 0;
            int rowCount = 0;

            for (int i = 0; i < rectChildren.Count; i++)
            {
                rowCount = i / columns;
                columnCount = i % columns;

                var item = rectChildren[i];

                var xPos = (cellSize.x * columnCount) + (spacing.x * columnCount) + padding.left;
                var yPos = (cellSize.y * rowCount) + (spacing.y * rowCount) + padding.top;

                SetChildAlongAxis(item, 0, xPos, cellSize.x);
                SetChildAlongAxis(item, 1, yPos, cellSize.y);
            }
        }

        public override void CalculateLayoutInputVertical()
        {
        }

        public override void SetLayoutHorizontal()
        {
        }

        public override void SetLayoutVertical()
        {
        }
    }
}
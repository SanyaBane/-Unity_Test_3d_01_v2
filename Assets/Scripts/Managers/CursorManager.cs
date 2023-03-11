using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Managers
{
    //public class CurrentCursorCondition
    //{
    //    bool CursorOverGUI;
    //}

    // https://youtu.be/8Fm37H1Mwxw
    public class CursorManager : MonoBehaviour
    {
        public enum CursorType
        {
            Normal,
            Sword
        }

        //public CurrentCursorCondition CurrentCursorCondition = new CurrentCursorCondition();

        private CursorData currentCursorData;

        [SerializeField] private List<CursorData> cursorDatas;

        private int currentFrame;
        private float frameTimer;
        private int frameCount;

        private void Start()
        {
            SetActiveCursorType(CursorType.Normal);
        }

        private void Update()
        {
            if(currentCursorData.TextureArray.Length > 1)
            {
                frameTimer -= Time.deltaTime;
                if (frameTimer <= 0f)
                {
                    frameTimer += currentCursorData.frameRate;
                    currentFrame = (currentFrame + 1) % frameCount;
                    Cursor.SetCursor(currentCursorData.TextureArray[currentFrame], currentCursorData.offset, CursorMode.Auto);
                }
            }
        }

        private void SetActiveCursorData(CursorData cursorData)
        {
            this.currentCursorData = cursorData;
            currentFrame = 0;

            if (cursorData.TextureArray.Length == 1)
            {
                Cursor.SetCursor(cursorData.TextureArray[currentFrame], cursorData.offset, CursorMode.Auto);
            }
            else
            {
                frameTimer = cursorData.frameRate; // 0
                frameCount = cursorData.TextureArray.Length;
            }
        }

        public void SetActiveCursorType(CursorType cursorType)
        {
            var cursorData = GetCursorData(cursorType);

            if (this.currentCursorData != null && this.currentCursorData.CursorType == cursorType)
                return;

            SetActiveCursorData(cursorData);
        }

        private CursorData GetCursorData(CursorType cursorType)
        {
            foreach (var cursorData in cursorDatas)
            {
                if (cursorData.CursorType == cursorType)
                    return cursorData;
            }

            return null;
        }

        [System.Serializable]
        public class CursorData
        {
            public CursorType CursorType;
            public Texture2D[] TextureArray;
            public float frameRate = 0.1f;
            public Vector2 offset;
        }
    }
}

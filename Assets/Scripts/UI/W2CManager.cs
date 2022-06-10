using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    public class W2CManager : MonoBehaviour
    {
        static W2CManager _instance;

        [SerializeField] RectTransform _canvasRect;
        [SerializeField] GameObject _damageBurst;

        Canvas _canvas;     
        RectTransform _rect;
        Camera _camera;

        void Awake()
        {
            _instance = this;
            _rect = GetComponent<RectTransform>();
            _canvas = _canvasRect.GetComponent<Canvas>();
            _camera = Camera.main;
        }

        public static void DoDamageBurst(int damage, Player player, Vector3 pos)
        {
            DamageBurst dburst = InstantiateAs<DamageBurst>(_instance._damageBurst);
            dburst.Init(damage, player.Team.TextColor);
            dburst.SetPosition(pos);
        }

        public static W2C Instantiate(GameObject prefab)
        {
            // Check we've set up the singleton first
            if (_instance == null)
            {
                return null;
            }

            // Instatiate the requested prefab, and get the W2C component
            GameObject spawnedObj = Instantiate(prefab, _instance._rect);
            W2C comp = spawnedObj.GetComponent<W2C>();

            // Immediately destroy the object if it's invalid
            if (comp == null)
            {
                Destroy(spawnedObj);
                return null;
            }

            // Assign the Canvas and Camera to it
            comp.Initialize(_instance._canvas, _instance._camera);

            return comp;
        }

        public static T InstantiateAs<T>(GameObject prefab) where T : W2C
        {
            return Instantiate(prefab) as T;
        }

        static Vector2 MousePositionToCanvas()
        {
            float width = _instance._canvasRect.sizeDelta.x;
            float height = _instance._canvasRect.sizeDelta.y;

            float x = Input.mousePosition.x / Screen.width;
            float y = Input.mousePosition.y / Screen.height;

            return new Vector2(x * width, y * height);
        }
    }

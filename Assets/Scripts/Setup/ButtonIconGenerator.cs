using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Setup {
    public class ButtonIconGenerator : MonoBehaviour
    {
        [System.Serializable]
        public struct ButtonData {
            public GameObject prefab;
            public RawImage uiTarget;
        }
        
        [SerializeField] private List<ButtonData> buttonList;
        [SerializeField] private Camera studioCamera;
        [SerializeField] private Light studioLight;
        [SerializeField] private Transform spawnPoint;
        
        void Start() {
            StartCoroutine(ProcessIconsRoutine());
            //MakeIcons();
        }
        
        private void MakeIcons() {
            RenderTexture rt = studioCamera.targetTexture;
            
            foreach (var buttonData in buttonList) {
                var instance = Instantiate(buttonData.prefab, spawnPoint.position, Quaternion.identity);
                
                studioCamera.Render();
                
                var stamp = new Texture2D(rt.width, rt.height, TextureFormat.RGBA32, false);
                
                RenderTexture.active = rt;
                stamp.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
                stamp.Apply();
                
                buttonData.uiTarget.texture = stamp;
                Destroy(instance);
                
            }
            
            studioCamera.gameObject.SetActive(false);
        }
        
        private IEnumerator ProcessIconsRoutine()
        {
            var rt = studioCamera.targetTexture;

            foreach (var item in buttonList)
            {
                var model = Instantiate(item.prefab, spawnPoint.position, spawnPoint.rotation);

                yield return new WaitForFrames(1);
                
                studioCamera.Render();

                var stamp = new Texture2D(rt.width, rt.height, TextureFormat.RGBA32, false);
                RenderTexture.active = rt;
                stamp.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
                stamp.Apply();

                item.uiTarget.texture = stamp;
                Destroy(model);
        
                yield return null; 
            }

            studioCamera.gameObject.SetActive(false);
            studioLight.gameObject.SetActive(false);
        }
    }
}

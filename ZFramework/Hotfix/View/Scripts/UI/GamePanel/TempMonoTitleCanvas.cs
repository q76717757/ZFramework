using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace ZFramework
{
    public class TempMonoTitleCanvas : MonoBehaviour
    {
        GameObject target;
        float t = 0;

        public void Init(GameObject roleModel, string msg)
        {
            this.target = roleModel;
            t = 5;
            GetComponent<References>().Get<TextMeshProUGUI>("msg").text = msg;
            gameObject.SetActive(true);
        }

        private void Update()
        {
            if (target != null)
            {
                if (t > 0)
                {
                    t -= Time.deltaTime;
                    transform.position = target.transform.position + Vector3.up * 2;
                    transform.rotation = Camera.main.transform.rotation;
                }
                else
                {
                    gameObject.SetActive(false);
                }
            }
            else
            {
                gameObject.SetActive(false);
            }
        }

    }
}

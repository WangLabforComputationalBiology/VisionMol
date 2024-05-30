using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;
using System.Collections;
using System.Collections.Generic;

namespace UMol
{
    public class dis : MonoBehaviour
    {
        public Camera mainCam;
        public List<UnityMolAtom> selectedAtoms = new List<UnityMolAtom>();
        private List<GameObject> haloObjects = new List<GameObject>();
        private List<GameObject> lineObjects = new List<GameObject>();
        private List<GameObject> highlightObjects = new List<GameObject>();

        private UnityMolSelectionManager selM;
        private bool showClearButton = true;

        public Slider redSlider;
        public Slider greenSlider;
        public Slider blueSlider;
        public Slider alphaSlider;

        private List<float> distances = new List<float>();

        public Text distanceText;
        private List<TextMesh> distanceTexts = new List<TextMesh>();

        public GameObject leftController;
        public GameObject rightController;
        public Button clear;

        void Start()
        {
            selM = UnityMolMain.getSelectionManager();
            mainCam = Camera.main;
        }

        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                UnityMolAtom a = getAtomPointed(leftController) ?? getAtomPointed(rightController);
                if (a != null)
                {
                    selM.selectionMode = UnityMolSelectionManager.SelectionMode.Atom;
                    selectedAtoms.Add(a);
                    if (selectedAtoms.Count % 2 == 0)
                    {
                        HighlightAtomg(a);
                        HighlightAtoms();
                        ConnectAtoms();
                    }
                    else
                    {
                        HighlightAtomy(a);
                    }
                }
            }

            UpdateLinePositions();
            UpdateLineColors();
            UpdateDistanceText();
        }

        UnityMolAtom getAtomPointed(GameObject controller)
        {
            Ray ray = new Ray(controller.transform.position, controller.transform.forward);
            CustomRaycastBurst raycaster = UnityMolMain.getCustomRaycast();
            return raycaster.customRaycastAtomBurst(ray.origin, ray.direction);
        }

        void UpdateLinePositions()
        {
            for (int i = 0; i < lineObjects.Count; i++)
            {
                GameObject lineGo = lineObjects[i];
                LineRenderer lineRenderer = lineGo.GetComponent<LineRenderer>();

                int haloIndex = i * 2;
                GameObject firstHaloGo = haloObjects[haloIndex];
                GameObject secondHaloGo = haloObjects[haloIndex + 1];

                lineRenderer.SetPosition(0, firstHaloGo.transform.position);
                lineRenderer.SetPosition(1, secondHaloGo.transform.position);
            }
        }

        void UpdateLineColors()
        {
            float red = redSlider.value;
            float green = greenSlider.value;
            float blue = blueSlider.value;
            float alpha = alphaSlider.value;
            Color newColor = new Color(red, green, blue, alpha);
            foreach (GameObject lineGo in lineObjects)
            {
                LineRenderer lineRenderer = lineGo.GetComponent<LineRenderer>();
                if (lineRenderer != null)
                {
                    lineRenderer.material.SetColor("_Color", newColor);
                }
            }
        }

        void HighlightAtomy(UnityMolAtom atom)
        {
            GameObject haloGo = CreateHighlightObject(atom, Color.yellow);
            highlightObjects.Add(haloGo);
        }

        void HighlightAtomg(UnityMolAtom atom)
        {
            GameObject haloGo = CreateHighlightObject(atom, Color.green);
            highlightObjects.Add(haloGo);
        }

        void HighlightAtoms()
        {
            for (int i = 0; i < selectedAtoms.Count; i += 2)
            {
                UnityMolAtom firstAtom = selectedAtoms[i];
                UnityMolAtom secondAtom = selectedAtoms[i + 1];

                GameObject firstHaloGo = CreateHighlightObject(firstAtom, Color.yellow);
                GameObject secondHaloGo = CreateHighlightObject(secondAtom, Color.green);

                haloObjects.Add(firstHaloGo);
                haloObjects.Add(secondHaloGo);
            }
        }

        void ConnectAtoms()
        {
            distances.Clear();
            for (int i = 0; i < selectedAtoms.Count; i += 2)
            {
                GameObject firstHaloGo = haloObjects[i * 2];
                GameObject secondHaloGo = haloObjects[i * 2 + 1];

                UnityMolAtom firstAtom = selectedAtoms[i];
                UnityMolAtom secondAtom = selectedAtoms[i + 1];

                float distance = Vector3.Distance(firstAtom.position, secondAtom.position);
                distances.Add(distance);

                GameObject lineGo = CreateLineObject(firstHaloGo.transform.position, secondHaloGo.transform.position, distance, firstAtom.position, secondAtom.position);
                lineObjects.Add(lineGo);
            }
        }

        GameObject CreateHighlightObject(UnityMolAtom atom, Color color)
        {
            GameObject haloGo = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            haloGo.name = "HaloSphere";
            haloGo.GetComponent<MeshRenderer>().material.color = color;

            UnityMolStructure structure = atom.residue.chain.model.structure;
            UnityMolStructureManager sm = UnityMolMain.getStructureManager();
            Transform molParent = sm.GetStructureGameObject(structure.uniqueName).transform;

            haloGo.transform.position = molParent.TransformPoint(atom.position);
            float scaleFactor = 0.013f;
            haloGo.transform.localScale = atom.radius * Vector3.one * scaleFactor;

            GameObject goAtom = structure.atomToGo[atom];
            haloGo.transform.SetParent(goAtom.transform);

            return haloGo;
        }

        GameObject CreateLineObject(Vector3 start, Vector3 end, float distance, Vector3 firstatom, Vector3 sencondatom)
        {
            GameObject lineGo = new GameObject("DashedLine");
            LineRenderer lineRenderer = lineGo.AddComponent<LineRenderer>();
            lineRenderer.positionCount = 2;
            lineRenderer.SetPositions(new Vector3[] { start, end });
            lineRenderer.startWidth = 0.005f;
            lineRenderer.endWidth = 0.005f;
            lineRenderer.material = new Material(Shader.Find("Unlit/Color"));
            lineRenderer.startColor = Color.white;
            lineRenderer.endColor = Color.white;

            UnityMolStructure structure = selectedAtoms[0].residue.chain.model.structure;
            UnityMolStructureManager sm = UnityMolMain.getStructureManager();
            Transform molParent = sm.GetStructureGameObject(structure.uniqueName).transform;
            lineGo.transform.SetParent(molParent, false);

            // 创建TextMesh对象
            GameObject textGo = new GameObject("DistanceText");
            TextMesh textMesh = textGo.AddComponent<TextMesh>();
            textMesh.text = $"{distance:F3} Å";
            textMesh.fontSize = 14;
            textMesh.anchor = TextAnchor.LowerCenter; // 设置文本锚点为下中心
            textMesh.alignment = TextAlignment.Center;
            textMesh.gameObject.AddComponent<LookAtCamera>();

            // 将TextMesh位置设置为线的中点上方
            Vector3 midpoint = (firstatom + sencondatom) / 2f;
            textGo.transform.position = midpoint;

            textGo.transform.SetParent(lineGo.transform, false);
            distanceTexts.Add(textMesh); // 将TextMesh添加到列表中

            return lineGo;
        }

        public void OnDestroy()
        {
            ClearHighlightsAndLines();
        }

        public void ClearHighlightsAndLines()
        {
            foreach (GameObject haloGo in haloObjects)
            {
                Destroy(haloGo);
            }
            haloObjects.Clear();

            foreach (GameObject lineGo in lineObjects)
            {
                Destroy(lineGo);
            }
            lineObjects.Clear();

            selectedAtoms.Clear();

            distances.Clear();
            foreach (TextMesh textMesh in distanceTexts)
            {
                Destroy(textMesh.gameObject);
            }
            distanceTexts.Clear();

            foreach (GameObject haloGo in highlightObjects)
            {
                Destroy(haloGo);
            }
            highlightObjects.Clear();
        }

        void UpdateDistanceText()
        {
            if (distanceText != null)
            {
                string distanceString = "";
                for (int i = 0; i < distances.Count; i++)
                {
                    distanceString += $"Distance {i + 1}: {distances[i]:F3} Å\n";
                }
                distanceText.text = distanceString;
            }
        }
    }
}

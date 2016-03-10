using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Node : MonoBehaviour {
    public Transform TextoTela;
    public List<Node> NosProximos;

    public Node ANode;

    public int NosVizinhos;

    public float G;

    public float H
    {
        get { //Fiz o custo baseado em cálculos de distância vetorial.
            return Vector3.Distance(AStar.Target.transform.position, transform.position);
        }
    }

    public float F
    {
        get
        {
            return G + H;
        }
    }

    MeshRenderer renderer = null; 
    public bool Parede;
	// Use this for initialization
	void Start () {
        renderer = GetComponent<MeshRenderer>();
        NosProximos = new List<Node>();
        for (int i = 0; i < transform.parent.childCount; i++)
        {
            var no = transform.parent.GetChild(i).GetComponent<Node>();

            if (no != this)
                NosProximos.Add(no);            
        }

        
        NosProximos.Sort((n1, n2) =>
        {
            var distanciaA = Vector3.Distance(n1.transform.position, transform.position);
            var distanciaB = Vector3.Distance(n2.transform.position, transform.position);
            if (distanciaA < distanciaB)
                return -1;
            else if (distanciaA == distanciaB)
                return 0;
            else
                return 1;
        });

        NosProximos.RemoveRange(NosVizinhos, NosProximos.Count - NosVizinhos);

        var rectTransform = TextoTela.GetComponent<RectTransform>();
        var textoTela = (Transform)Instantiate(TextoTela);
        textoTela.SetParent(transform);
        textoTela.GetComponent<RectTransform>().localPosition = rectTransform.localPosition;
        textoTela.GetComponent<RectTransform>().localRotation = rectTransform.localRotation;
        textoTela.GetComponent<RectTransform>().localScale = rectTransform.localScale;

        textoTela.GetComponentInChildren<Text>().text = transform.name;
        
	}
    bool MouseUp = false;    
	// Update is called once per frame
	void Update () {
        if (MouseUp) return;
        renderer.material = AStar.Current.Normal;

        if (this == AStar.Current.FinalNode)
            renderer.material = AStar.Current.Fim;        

        if (Parede)
            renderer.material = AStar.Current.Parede;

        if (AStar.Current.sucessores != null && AStar.Current.sucessores.Exists(p => p == this))
            renderer.material = AStar.Current.Sucessores;       

        if (AStar.Current.currentNode == this)
            renderer.material = AStar.Current.Atual;

        if (this == AStar.Current.StartNode)
            renderer.material = AStar.Current.Inicio;

        if (this == AStar.Current.vizinho)
            renderer.material = AStar.Current.SucessorAtual;

        if (AStar.Current.Path.Exists(p => p == this))
            renderer.material = AStar.Current.RotaFinal;

        
	}    

    void OnMouseEnter()
    {

        MouseUp = true;
        renderer.material = AStar.Current.MouseOver;        
    }

    void OnMouseOver()
    {
        renderer.material = AStar.Current.MouseOver;
        if (Input.GetMouseButton(0))
            if (Input.GetKey(KeyCode.LeftControl))
                Parede = true;
            else
                AStar.Current.StartNode = this;
        else if (Input.GetMouseButton(1))
        {
            if (Input.GetKey(KeyCode.LeftControl))
                Parede = false;
            else
                AStar.Current.FinalNode = this;
        }        
        
    }

    void OnMouseExit()
    {
        MouseUp = false;        
    }    

    public override string ToString()
    {
        return transform.name;
    }
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class AStar : MonoBehaviour {
    public Vector3 Destino;

    public Node StartNode;
    public Node FinalNode;
    public Node currentNode;

    UniqueList<Node> Open;
    UniqueList<Node> Close;
    public UniqueList<Node> Path;

    public static Node Target;

    public Material Normal;
    public Material Atual;
    public Material Inicio;
    public Material MouseOver;
    public Material Fim;
    public Material Parede;
    public Material Passou;
    public Material RotaFinal;
    public Material Sucessores;
    public Material SucessorAtual;

    public Transform PassosPanel;
    public Text Passo1;
    public Text Passo2;
    public Text Passo2a;
    public Text Passo2b;
    public Text Passo2c;
    public Text Passo2cI;
    public Text Passo2cII;
    public Text Passo2cII1;
    public Text Passo2cII2;
    public Text Passo2d;
    public Text Passo2dI;
    public Text Passo2dII;
    public Text Passo3;   

    private int passo;

    public Text NoAtualText;
    public Text ListaOpenText;
    public Text ListaCloseText;

    public Button CalcularTudoBotao;

    
    public static AStar Current;    

	// Use this for initialization
	void Start () {
        Current = this;
        Open = new UniqueList<Node>();
        Close = new UniqueList<Node>();
        Path = new UniqueList<Node>();
	}
	
	// Update is called once per frame
    Node buscaMenorCustoF(UniqueList<Node> lista)
    {
        Node node = null;

        foreach (var no in lista)
        {
            if (node == null || no.F < node.F)
                node = no;
        }
        return node;
    }
	
    void Update()
    {
        Passo1.fontStyle     = passo == 1 ? FontStyle.Bold  : FontStyle.Normal;
        Passo2.fontStyle     = passo == 2 ? FontStyle.Bold  : FontStyle.Normal;
        Passo2a.fontStyle    = passo == 3 ? FontStyle.Bold  : FontStyle.Normal;
        Passo2b.fontStyle    = passo == 4 ? FontStyle.Bold  : FontStyle.Normal;
        Passo2c.fontStyle    = passo == 5 ? FontStyle.Bold  : FontStyle.Normal;
        Passo2cI.fontStyle   = passo == 6 ? FontStyle.Bold  : FontStyle.Normal;
        Passo2cII.fontStyle  = passo == 7 ? FontStyle.Bold  : FontStyle.Normal;
        Passo2cII1.fontStyle = passo == 8 ? FontStyle.Bold  : FontStyle.Normal;
        Passo2cII2.fontStyle = passo == 9 ? FontStyle.Bold  : FontStyle.Normal;
        Passo2d.fontStyle    = passo == 11 ? FontStyle.Bold : FontStyle.Normal;
        Passo2dI.fontStyle   = passo == 12 ? FontStyle.Bold : FontStyle.Normal;
        Passo2dII.fontStyle  = passo == 13 ? FontStyle.Bold : FontStyle.Normal;
        Passo3.fontStyle     = passo == 14 ? FontStyle.Bold : FontStyle.Normal;

        if (currentNode != null)
            NoAtualText.text = currentNode.gameObject.name;

        string listaOpen = "";
        foreach (var node in Open)
            listaOpen += string.IsNullOrEmpty(listaOpen) ? node.gameObject.name : ", " + node.gameObject.name;
        ListaOpenText.text = listaOpen;

        string listaClose = "";
        foreach (var node in Close)
            listaClose += string.IsNullOrEmpty(listaClose) ? node.gameObject.name : ", " + node.gameObject.name;
        ListaCloseText.text = listaClose;
    }
        
    public List<Node> sucessores;
    public Node vizinho;
    private void VerificaClose()
    {
        vizinho = sucessores[0];
        sucessores.Remove(vizinho);

        if (Close.Exists(p => p == vizinho)) {
            sucessores.Remove(vizinho);
            if (sucessores.Count > 0)
                passo--;
            else
                passo = 2;            
        }
    }

    private void EstaEmOpen()
    {
        
        //Node vizinho = sucessores[0];        
        if (Open.Exists(p => p == vizinho))
        {
            var distancia = (currentNode.G + Vector3.Distance(currentNode.transform.position, vizinho.transform.position));
            if (vizinho.G >= distancia)
            {
                vizinho.G = distancia;
                vizinho.ANode = currentNode;
            }
        }        
    }


    void NaoEstaEmOpen()
    {        
        //Node vizinho = sucessores[0];        
        //sucessores.Remove(vizinho);
        passo = 5;
        if (!Open.Exists(p => p == vizinho))
        {
            Open.Add(vizinho);
            vizinho.ANode = currentNode;
            vizinho.G = currentNode.G + Vector3.Distance(vizinho.transform.position, currentNode.transform.position);
        }
        
        if (sucessores.Count == 0) //Se passou por todos os vizinhos volta pro passo 2;
        {
            passo = 2;
            return;
        }
    }
        
    void Continuar()
    {         
                
        if ((currentNode != FinalNode && (Open.Count > 0 || (sucessores != null && sucessores.Count > 0)) || currentNode == StartNode) || passo == 1) {
            switch (passo)
            {
                case 1:
                    Open.Add(StartNode);
                    break;
                case 3:
                    currentNode = buscaMenorCustoF(Open);
                    break;
                case 4:                    
                    Close.Add(currentNode);
                    Open.Remove(currentNode);
                    //Open.AddRange(currentNode.NosProximos.FindAll(p => p.Parede == false));
                    sucessores = null;
                    break;
                case 5:                    
                    sucessores = currentNode.NosProximos.FindAll(p => p.Parede == false); // Busca todos os nós vizinhos que não sejam paredes.                            
                    vizinho = null;
                    break;
                case 6:
                    VerificaClose();
                    break;
                case 8:
                    EstaEmOpen();
                    break;
                case 9:
                    NaoEstaEmOpen();
                    break;                         
            }
        }
        else if (passo < 11)        
            passo = 11;           
        else
        {   
            if (passo < 13) {
                if (currentNode == Target)            
                    passo = 12;
                else            
                    passo = 13;
            }
            else {
                passo = 14;
                currentNode = null;
                this.vizinho = null;
                var node = FinalNode.ANode;
                if (node != null)
                {
                    do
                    {
                        Path.Add(node);
                        node = node.ANode;
                    } while (node != null);
                }
            }
        }

        passo++;
            
    }

    void CalcularTudo()
    {
        CalcularTudoBotao.interactable = false;
        Open.Clear();
        Close.Clear();
        Path.Clear();

        if (StartNode == null || FinalNode == null)
        {
            Debug.Log("Selecione a origem e o destino");
            return;
        }

        passo = 1;
        Target = FinalNode;
        sucessores = null;

        Open.Add(StartNode);
        currentNode = StartNode;
        currentNode.G = 0;
        while (currentNode != FinalNode && Open.Count > 0)
        {

            currentNode = buscaMenorCustoF(Open);
            Open.Remove(currentNode);
            Close.Add(currentNode);

            sucessores = currentNode.NosProximos.FindAll(p => p.Parede == false); // Busca todos os nós vizinhos que não sejam paredes.
            foreach (var vizinho in sucessores)
            {
                if (Close.Exists(p => p == vizinho))
                    continue;

                if (Open.Exists(p => p == vizinho))
                {
                    var distancia = (currentNode.G + Vector3.Distance(currentNode.transform.position, vizinho.transform.position));
                    if (vizinho.G >= distancia)
                    {
                        vizinho.G = distancia;
                        vizinho.ANode = currentNode;
                    }
                }
                else
                {
                    Open.Add(vizinho);
                    vizinho.ANode = currentNode;
                    vizinho.G = currentNode.G + Vector3.Distance(vizinho.transform.position, currentNode.transform.position);
                }
            }
        }

        //Inicia percurso de traz pra frente e grava na lista Path.
        if (currentNode == Target || Open.Count == 0)
        {
            var node = FinalNode.ANode;
            if (node != null)
            {
                do
                {
                    Path.Add(node);
                    node = node.ANode;
                } while (node != null);
            }
        }

        sucessores = null;
        this.vizinho = null;
        currentNode = null;
    }

    void Calcular()
    {
        CalcularTudoBotao.interactable = false;
        Open.Clear();
        Close.Clear();
        Path.Clear();

        if (StartNode == null || FinalNode == null)
        {
            Debug.Log("Selecione a origem e o destino");
            return;
        }

        passo = 1;
        Target = FinalNode;
        sucessores = null;
        PassosPanel.gameObject.SetActive(true);
    }

    void Limpar()
    {
        CalcularTudoBotao.interactable = true;
        PassosPanel.gameObject.SetActive(false);
        this.StartNode = null;
        this.FinalNode = null;
        this.currentNode = null;
        Path.Clear();
        Open.Clear();
        Close.Clear();
        sucessores = null;

        foreach (Node node in GetComponentsInChildren<Node>()) { 
            node.Parede = false;
            node.ANode = null;
            node.G = 0;
        }     
        
            
    }
    
}

public class UniqueList<T> : List<T>
{
    public new void Add(T obj)
    {
        if (!Contains(obj))
        {
            base.Add(obj);
        }
    }
}

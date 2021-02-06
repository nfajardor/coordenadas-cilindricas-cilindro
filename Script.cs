using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script : MonoBehaviour
{
    // Start is called before the first frame update

    //Declaracion de variables
    float r = 2f;                           //Coordenada del radio del origen
    float t = (float)(System.Math.PI/2);    //Coordenada del angulo del origen
    float h = 0f;                           //Coordenada de la altura del origen
    float radio = 1f;                       //Radio del cilindro
    float altura = 4f;                      //Altura del cilindro
    Vector3[] vertices;                     //Vetrices del mesh
    int[] triangulos;                       //Triangulos del mesh
    Mesh mesh;                              //El mesh
    GameObject obj;                         //El GameObject
    Vector3 origen;                         //Coordenadas del triangulo
    int vPerim = 360;                       //Cantidad de vertices en el perimetro de uno de los circulos

    void Start()
    {
        //Creacion de el GameObject y la asignacion del mesh que estamos creando
        obj = new GameObject("obj", typeof(MeshFilter), typeof(MeshRenderer));
        mesh = new Mesh();
        obj.GetComponent<Renderer>().material.color = Color.red;
        obj.GetComponent<MeshFilter>().mesh = mesh;
        //Metodos para hacer el cilindro
        centrar();
        vPolares();
        vCartesianos();
        asignarTriangulos();
        dibujarMesh();
    }


    //Dibuja el mesh
    void dibujarMesh() {
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangulos;
        mesh.RecalculateNormals();
    }
  
    /*
     * Toma cambia las coordenadas de polares a cartesianas y las asigna al Vector3 origen
     */
    void centrar() {
        float x = (float)(r*System.Math.Cos(t));
        float y = h;
        float z = (float)(r*System.Math.Sin(t));
        origen = new Vector3(x,y,z);
    }

    /*
     * Asigna a vertices todos los vertices necesarios para crear el poligono (en polares)
     */
    void vPolares() {
        vertices = new Vector3[2*vPerim+2];
        vertices[0] = new Vector3(0,0,altura/2);
        vertices[1] = new Vector3(0,0,-altura/2);
        float delta = (float)(2*System.Math.PI/vPerim);
        for (int i = 2; i < (vPerim+2); i++){
            vertices[i] = new Vector3(radio,vertices[i-1].y+delta,altura/2);
        }
        for(int i = (vPerim+2);i<vertices.Length;i++){
            vertices[i] = new Vector3(radio,vertices[i-1].y+delta,-altura/2);
        }
    }

    /*
     * Cambia las coordenadas de los vertices en vertices de polares a cartesianas, realizando el ajuste para el origen
     */
    void vCartesianos() {
        for (int i = 0; i < vertices.Length; i++){
            float x = (float)((vertices[i].x)*(System.Math.Cos(vertices[i].y)));
            float z = (float)((vertices[i].x)*(System.Math.Sin(vertices[i].y)));
            float y = (float)(vertices[i].z);
            x += origen.x;
            y += origen.y;
            z += origen.z;
            vertices[i] = new Vector3(x,y,z);
        }
    }

    /*
     * Ajusta el orden de los vertices para crear los traingulos
     */
    void asignarTriangulos() {
        triangulos = new int[4*vPerim*3];
        int inicio = vPerim+1;
        int trigs = 1;
        //Ajusta los triangulos del circulo superior
        for(int i = 0;i<vPerim*3;i++){
            if(i%3==0){
                triangulos[i] = 0;
            }
            else if(i%3==1){
                triangulos[i] = inicio;
                inicio--;
            }
            else {
                if(inicio == (1)){
                    inicio = vPerim+1;
                }
                triangulos[i] = inicio;
                Debug.Log("Agregado triangulo " + trigs + ": " + triangulos[i-2] + ", " + triangulos[i-1] + ", " + triangulos[i]);
                trigs++;
            }
        }

        inicio = vPerim+2;
        //Ajusta los triangulos del circulo inferior
        for(int i = vPerim*3; i < 2*(vPerim*3);i++){
            if(i%3==0){
                triangulos[i] = 1;
            }
            else if(i%3==1){
                triangulos[i] = inicio;
                inicio++;
            }
            else {
                if(inicio == (vertices.Length)){
                    inicio = vPerim+2;
                }
                triangulos[i] = inicio;
                Debug.Log("Agregado triangulo " + trigs + ": " + triangulos[i-2] + ", " + triangulos[i-1] + ", " + triangulos[i]);
                trigs++;
            }
        }
        int iSup = 2;
        int iInf = vPerim+2;
        int j = 2*3*vPerim;

        //Llena todos menos uno de los triangulos laterales con base en la parte inferior del cilindro
        for(int i = 0; i<vPerim-1;i++){
            //A
            triangulos[j] = iSup;
            j++;
            //B
            triangulos[j] = iInf+1;
            j++;
            //C
            triangulos[j] = iInf;
            j++;
            iSup++;
            iInf++;
        }
        iSup = 2;
        iInf = vPerim+3;
        for(int i = 0; i<vPerim-1;i++){
            //A
            triangulos[j] = iSup;
            j++;
            //B
            triangulos[j] = iSup+1;
            j++;
            //C
            triangulos[j] = iInf;
            j++;

            iSup++;
            iInf++;
        }

        //Se agregan los ultimos dos triangulos
        
        //A
        triangulos[j] = vPerim+1;
        j++;
        //B
        triangulos[j] = vPerim+2;
        j++;
        //C
        triangulos[j] = vertices.Length-1;
        j++;
        //D
        triangulos[j] = vPerim+1;
        j++;
        //E
        triangulos[j] = 2;
        j++;
        //F
        triangulos[j] = vPerim+2;
        j++;
    }
}

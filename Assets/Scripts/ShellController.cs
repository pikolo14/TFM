using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShellController : MonoBehaviour
{
    [HideInInspector]
    public Collider coll;
    [HideInInspector]
    public Rigidbody rb;


    //Stats
    public int health;
    public int weight;
    public float size;

    //Rango de tama�os que admite la concha
    public float minSize;
    public float maxSize;

    public Vector3 initScale;

    //Tiempo que se ignora al cangrejo despues de soltar la concha
    public static float ignoreCollTime = 3;
    public GameObject ignoredCrab;

    //Punto del cangrejo en el que se debe colocar continuamente (evitamos escalar la concha con el cangrejo)
    private Transform anchorPoint;



    // Start is called before the first frame update
    void Start()
    {
        coll = GetComponent<Collider>();
        rb = GetComponent<Rigidbody>();
        initScale = transform.localScale;

        //Si no esta sobre un cangrejo se inicializa su tama�o aleatoriamente (si no se asigna manualmente)
        if(anchorPoint == null)
        {
            //Inicializar con un tama�o aleatorio
            float ranSize = Random.Range(GameManager.gameManager.minShellSize, GameManager.gameManager.maxShellSize);
            SetSize(ranSize);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(anchorPoint != null)
        {
            transform.position = anchorPoint.position;
        }
    }

    public void SetSize(float _size)
    {
        size = _size;
        minSize = Mathf.Max(size - GameManager.gameManager.shellSizeTolerance, 0);
        maxSize = size + GameManager.gameManager.shellSizeTolerance;

        //Actualizar escala con ese tama�o
        transform.localScale = initScale * size * GameManager.gameManager.scaleFactor;
    }


    public float GetDisconfort(float crabSize)
    {
        //Si el cangrejo es demasiado peque�o devolvemos un valor negativo con lo que le falta
        float underSize = (crabSize - minSize) * CrabController.disconfortFactor;
        if (underSize < 0)
            return underSize;

        //Si el cangrejo es demasiado grande devolvemos un valor positivo con lo que le sobra
        float overSize = (crabSize - maxSize) * CrabController.disconfortFactor;
        if(overSize > 0)
            return overSize;

        return 0;
    }

    //Vincular concha al cangrejo poniendola en su espalda
    public bool PickUp(Transform parent, Transform point)
    {
        if (ignoredCrab == null || ignoredCrab != parent.gameObject)
        {
            coll.enabled = false;
            rb.isKinematic = true;
            transform.parent = parent;
            anchorPoint = point;
            //transform.position = point.transform.position;
            transform.localRotation = point.transform.localRotation;
            return true;
        }
        else
            return false;
    }

    //Desvinculamos la concha de su cangrejo e ignoramos sus colisiones temporalmente
    public void Drop()
    {
        StartCoroutine(IgnoreTemporarily(transform.parent.gameObject, ignoreCollTime));

        coll.enabled = true;
        rb.isKinematic = false;
        transform.parent = null;
        anchorPoint = null;
    }

    //Ignorar colisiones con un objeto temporalmente
    public IEnumerator IgnoreTemporarily(GameObject ignored, float time)
    {
        //Collider ignoredColl = ingored.GetComponent<Collider>();
        //Physics.IgnoreCollision(coll, ignoredColl, true);
        ignoredCrab = ignored;
        yield return new WaitForSeconds(time);
        ignoredCrab = null;
        //Physics.IgnoreCollision(coll, ingored.GetComponent<Collider>(), false);
    }
}

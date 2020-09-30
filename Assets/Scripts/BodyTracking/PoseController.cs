using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PoseController : MonoBehaviour
{
    public Vector3 hombroDerecho;
    public Vector3 codoDerecho;
    public Vector3 manoDerecha;

    public Vector3 hombroIzquierdo;
    public Vector3 codoIzquierdo;
    public Vector3 manoIzquierda;

    public PoseController() { }
    /*
    public void ActualizarCoordenadas(
        Vector3 _hombroDerecho,
        Vector3 _codoDerecho,
        Vector3 _manoDerecha,
        Vector3 _hombroIzquierdo,
        Vector3 _codoIzquierdo,
        Vector3 _manoIzquierda
        )
    {
        hombroDerecho = _hombroDerecho;
        codoDerecho = _codoDerecho;
        manoDerecha = _manoDerecha;
        hombroIzquierdo = _hombroIzquierdo;
        codoIzquierdo = _codoIzquierdo;
        manoIzquierda = _manoIzquierda;
    }
    */
    public double CalcularAnguloHombroCodoIzquierda(Vector3 hombro,Vector3 codo)
    {
        double anguloRefCodo,resultado=0;

        Vector3 referenciaHombroCodo = new Vector3(codo.x, hombro.y, codo.z);

        float distanciaReferenciaCodo = Vector3.Distance(referenciaHombroCodo, codo);
        float distanciaHombroCodo = Vector3.Distance(hombro, codo);
        anguloRefCodo = (180/Mathf.PI) * Mathf.Asin(distanciaReferenciaCodo / distanciaHombroCodo);
        //Verificado
        if (hombro.y > codo.y && hombro.x < codo.x)
            resultado = 90 - anguloRefCodo;
        //Verificado
        else if (hombro.y < codo.y && hombro.x < codo.x)
            resultado = 90 + anguloRefCodo;

        return resultado;
    }

    public double CalcularAnguloCodoManoIzquierda(Vector3 codo, Vector3 mano, double anguloHombroCodo)
    {
        double anguloRefMano,resultado=0;

        Vector3 referenciaCodoMano = new Vector3(codo.x, mano.y, codo.z);

        float distanciaReferenciaMano = Vector3.Distance(referenciaCodoMano, mano);
        float distanciaCodoMano = Vector3.Distance(codo, mano);
        anguloRefMano = (180 / Mathf.PI) * Mathf.Asin(distanciaReferenciaMano / distanciaCodoMano);
        //Verificado
        if (codo.y < mano.y && codo.x < mano.x)
            resultado = anguloHombroCodo + anguloRefMano;
        //Verificado
        else if (codo.y < mano.y && codo.x > mano.x)
            resultado = anguloHombroCodo - anguloRefMano;

        else if (codo.y > mano.y && codo.x < mano.x)
            resultado = 180 + anguloHombroCodo - anguloRefMano;

        else if (codo.y > mano.y && codo.x > mano.x)
            resultado = 180 + anguloHombroCodo + anguloRefMano;

        return resultado;
    }

    public double CalcularAnguloHombroCodoDerecha(Vector3 hombro, Vector3 codo)
    {
        double anguloRefCodo, resultado = 0;

        Vector3 referenciaHombroCodo = new Vector3(codo.x, hombro.y, codo.z);

        float distanciaReferenciaCodo = Vector3.Distance(referenciaHombroCodo, codo);
        float distanciaHombroCodo = Vector3.Distance(hombro, codo);
        anguloRefCodo = (180 / Mathf.PI) * Mathf.Asin(distanciaReferenciaCodo / distanciaHombroCodo);
        //Verificado
        if (hombro.y > codo.y && hombro.x > codo.x)
            resultado = 90 - anguloRefCodo;
        //Verificado
        else if (hombro.y < codo.y && hombro.x > codo.x)
            resultado = 90 + anguloRefCodo;

        return resultado;
    }
    public double CalcularAnguloCodoManoDerecha(Vector3 codo, Vector3 mano, double anguloHombroCodo)
    {
        double anguloRefMano, resultado = 0;

        Vector3 referenciaCodoMano = new Vector3(codo.x, mano.y, codo.z);

        float distanciaReferenciaMano = Vector3.Distance(referenciaCodoMano, mano);
        float distanciaCodoMano = Vector3.Distance(codo, mano);
        anguloRefMano = (180 / Mathf.PI) * Mathf.Asin(distanciaReferenciaMano / distanciaCodoMano);
        //Verificado
        if (codo.y < mano.y && codo.x > mano.x)
            resultado = anguloHombroCodo + anguloRefMano;
        //Verificado
        else if (codo.y < mano.y && codo.x < mano.x)
            resultado = anguloHombroCodo - anguloRefMano;

        else if (codo.y > mano.y && codo.x > mano.x)
            resultado = 180 + anguloHombroCodo - anguloRefMano;

        else if (codo.y > mano.y && codo.x < mano.x)
            resultado = 180 + anguloHombroCodo + anguloRefMano;

        return resultado;
    }

    public Posicion IdentificarPosicion(Vector3 hombroD, Vector3 codoD, Vector3 manoD, Vector3 hombroI, Vector3 codoI, Vector3 manoI)
    {
        double anguloD1, anguloD2, anguloI1, anguloI2;

        anguloD1 = CalcularAnguloHombroCodoDerecha(hombroD, codoD);
        anguloD2 = CalcularAnguloCodoManoDerecha(codoD, manoD, anguloD1);

        anguloI1 = CalcularAnguloHombroCodoIzquierda(hombroI, codoI);
        anguloI2 = CalcularAnguloCodoManoIzquierda(codoI, manoI, anguloI1);


        if (95 <= anguloD1 && anguloD1 <= 115 &&
            60 <= anguloD2 && anguloD2 <= 80 &&
            95 <= anguloI1 && anguloI1 <= 115 &&
            130 <= anguloI2 && anguloI2 <= 150)
            return Posicion.Sonido1;

        else if (60 <= anguloD1 && anguloD1 <= 80 &&
            200 <= anguloD2 && anguloD2 <= 220 &&
            60 <= anguloI1 && anguloI1 <= 75 &&
            200 <= anguloI2 && anguloI2 <= 220)
            return Posicion.Sonido2;

        else if (100 <= anguloD1 && anguloD1 <= 130 &&
            100 <= anguloD2 && anguloD2 <= 150 &&
            50 <= anguloI1 && anguloI1 <= 80 &&
            200 <= anguloI2 && anguloI2 <= 260)
            return Posicion.Sonido3;

        else if (50 <= anguloD1 && anguloD1 <= 70 &&
            260 <= anguloD2 && anguloD2 <= 290 &&
            110 <= anguloI1 && anguloI1 <= 130 &&
            100 <= anguloI2 && anguloI2 <= 160)
            return Posicion.Sonido4;

        else if (85 <= anguloD1 && anguloD1 <= 105 &&
            170 <= anguloD2 && anguloD2 <= 190 &&
            40 <= anguloI1 && anguloI1 <= 60 &&
            -20 <= anguloI2 && anguloI2 <= -5)
            return Posicion.Sonido5;

        else if (40 <= anguloD1 && anguloD1 <= 70 &&
            -20 <= anguloD2 && anguloD2 <= -5 &&
            85 <= anguloI1 && anguloI1 <= 105 &&
            170 <= anguloI2 && anguloI2 <= 190)
            return Posicion.Sonido6;

        else if (80 <= anguloD1 && anguloD1 <= 100 &&
            170 <= anguloD2 && anguloD2 <= 190 &&
            40 <= anguloI1 && anguloI1 <= 65 &&
            250 <= anguloI2 && anguloI2 <= 290)
            return Posicion.Sonido7;

        else if (40 <= anguloD1 && anguloD1 <= 65 &&
            250 <= anguloD2 && anguloD2 <= 290 &&
            80 <= anguloI1 && anguloI1 <= 100 &&
            170 <= anguloI2 && anguloI2 <= 190)
            return Posicion.Sonido8;

        else if (80 <= anguloD1 && anguloD1 <= 110 &&
            20 <= anguloD2 && anguloD2 <= 150 &&
            80 <= anguloI1 && anguloI1 <= 100 &&
            170 <= anguloI2 && anguloI2 <= 190)
            return Posicion.SubirVolumen;

        else if (80 <= anguloD1 && anguloD1 <= 110 &&
            210 <= anguloD2 && anguloD2 <= 300 &&
            80 <= anguloI1 && anguloI1 <= 110 &&
            170 <= anguloI2 && anguloI2 <= 190)
            return Posicion.BajarVolumen;

        else if (80 <= anguloD1 && anguloD1 <= 110 &&
            170 <= anguloD2 && anguloD2 <= 190 &&
            80 <= anguloI1 && anguloI1 <= 100 &&
            20 <= anguloI2 && anguloI2 <= 150)
            return Posicion.Acelerar;

        else if (80 <= anguloD1 && anguloD1 <= 110 &&
            170 <= anguloD2 && anguloD2 <= 190 &&
            80 <= anguloI1 && anguloI1 <= 110 &&
            210 <= anguloI2 && anguloI2 <= 300)
            return Posicion.Ralentizar;

        else if (40 <= anguloD1 && anguloD1 <= 60 &&
            -25 <= anguloD2 && anguloD2 <= 25 &&
            40 <= anguloI1 && anguloI1 <= 60 &&
            -25 <= anguloI2 && anguloI2 <= 25)
            return Posicion.Finalizar;

        else if (120 <= anguloD1 && anguloD1 <= 150 &&
            80 <= anguloD2 && anguloD2 <= 110 &&
            120 <= anguloI1 && anguloI1 <= 150 &&
            80 <= anguloI2 && anguloI2 <= 110)
            return Posicion.Reiniciar;

        else
            return Posicion.Ninguna;
    }


    public bool ResumePosition(JointTracker[] joints, AudioClip audioClip)
    {
        JointTracker leftHandJoint = joints.Where(x => x.gameObject.transform.parent.name == "LeftHand").SingleOrDefault();
        JointTracker rigthHandJoint = joints.Where(x => x.gameObject.transform.parent.name == "RightHand").SingleOrDefault();
        JointTracker rootJoint = joints.Where(x => x.gameObject.transform.parent.name == "Root").SingleOrDefault();

        if (leftHandJoint == null || rigthHandJoint == null || rootJoint == null)
            return false;
        else
        {
            if (rigthHandJoint.transform.position.x - rootJoint.transform.position.x <= 0 &&
                rigthHandJoint.transform.position.x - rootJoint.transform.position.x >= -0.3 &&
                leftHandJoint.transform.position.x - rootJoint.transform.position.x >= 0 &&
                leftHandJoint.transform.position.x - rootJoint.transform.position.x <= 0.3 &&
                rigthHandJoint.transform.position.x - leftHandJoint.transform.position.x >= -0.3 &&
                rigthHandJoint.transform.position.x - leftHandJoint.transform.position.x <= 0 &&
                leftHandJoint.transform.position.y - rigthHandJoint.transform.position.y >= -0.1 &&
                leftHandJoint.transform.position.y - rigthHandJoint.transform.position.y <= 0.1 &&
                leftHandJoint.transform.position.y - rootJoint.transform.position.y >= 0.8 &&
                rigthHandJoint.transform.position.y - rootJoint.transform.position.y >= 0.8)
                return true;
            else
                return false;             
        }
    }
    public bool PausePosition(JointTracker[] joints)
    {
        JointTracker leftHandJoint = joints.Where(x => x.gameObject.transform.parent.name == "LeftHand").SingleOrDefault();
        JointTracker rigthHandJoint = joints.Where(x => x.gameObject.transform.parent.name == "RightHand").SingleOrDefault();
        JointTracker rootJoint = joints.Where(x => x.gameObject.transform.parent.name == "Root").SingleOrDefault();

        if (leftHandJoint == null || rigthHandJoint == null || rootJoint == null)
            return false;
        else
        {
            
            if (rigthHandJoint.transform.position.x - rootJoint.transform.position.x <= -0.3 &&
                rigthHandJoint.transform.position.x - rootJoint.transform.position.x >= -0.5 &&
                leftHandJoint.transform.position.x - rootJoint.transform.position.x >= 0.3 &&
                leftHandJoint.transform.position.x - rootJoint.transform.position.x <= 0.5 &&
                rigthHandJoint.transform.position.x - leftHandJoint.transform.position.x >= -1.0 &&
                rigthHandJoint.transform.position.x - leftHandJoint.transform.position.x <= -0.3 &&
                leftHandJoint.transform.position.y - rigthHandJoint.transform.position.y >= -0.1 &&
                leftHandJoint.transform.position.y - rigthHandJoint.transform.position.y <= 0.1 &&
                leftHandJoint.transform.position.y - rootJoint.transform.position.y >= 0.8 &&
                rigthHandJoint.transform.position.y - rootJoint.transform.position.y >= 0.8)
                return true;
            else
                return false;
        }
    }
    public bool FinishPosition(JointTracker[] joints, AudioClip audioClip)
    {
        JointTracker leftHandJoint = joints.Where(x => x.gameObject.transform.parent.name == "LeftHand").SingleOrDefault();
        JointTracker rigthHandJoint = joints.Where(x => x.gameObject.transform.parent.name == "RightHand").SingleOrDefault();
        JointTracker rootJoint = joints.Where(x => x.gameObject.transform.parent.name == "Root").SingleOrDefault();

        if (leftHandJoint == null || rigthHandJoint == null || rootJoint == null)
            return false;
        else
        {

            if (rigthHandJoint.transform.position.x - rootJoint.transform.position.x <= -0.3 &&
                rigthHandJoint.transform.position.x - rootJoint.transform.position.x >= -0.5 &&
                leftHandJoint.transform.position.x - rootJoint.transform.position.x >= 0.3 &&
                leftHandJoint.transform.position.x - rootJoint.transform.position.x <= 0.5 &&
                rigthHandJoint.transform.position.x - leftHandJoint.transform.position.x >= -1.0 &&
                rigthHandJoint.transform.position.x - leftHandJoint.transform.position.x <= -0.5 &&
                leftHandJoint.transform.position.y - rigthHandJoint.transform.position.y >= -1.2 &&
                leftHandJoint.transform.position.y - rigthHandJoint.transform.position.y <= -0.9 &&
                leftHandJoint.transform.position.y - rootJoint.transform.position.y <= 0.1 &&
                rigthHandJoint.transform.position.y - rootJoint.transform.position.y >= 0.8)
                return true;
            else
                return false;
        }
    }
    public bool RestartPosition(JointTracker[] joints)
    {
        JointTracker leftHandJoint = joints.Where(x => x.gameObject.transform.parent.name == "LeftHand").SingleOrDefault();
        JointTracker rigthHandJoint = joints.Where(x => x.gameObject.transform.parent.name == "RightHand").SingleOrDefault();
        JointTracker rootJoint = joints.Where(x => x.gameObject.transform.parent.name == "Root").SingleOrDefault();

        if (leftHandJoint == null || rigthHandJoint == null || rootJoint == null)
            return false;
        else
        {

            if (rigthHandJoint.transform.position.x - rootJoint.transform.position.x <= 0.2 &&
                rigthHandJoint.transform.position.x - rootJoint.transform.position.x >= -0.2 &&
                leftHandJoint.transform.position.x - rootJoint.transform.position.x >= -0.2 &&
                leftHandJoint.transform.position.x - rootJoint.transform.position.x <= 0.2 &&
                rigthHandJoint.transform.position.x - leftHandJoint.transform.position.x >= -0.4 &&
                rigthHandJoint.transform.position.x - leftHandJoint.transform.position.x < -0.1 &&
                leftHandJoint.transform.position.y - rigthHandJoint.transform.position.y >= -0.1 &&
                leftHandJoint.transform.position.y - rigthHandJoint.transform.position.y <= 0.1 &&
                leftHandJoint.transform.position.y - rootJoint.transform.position.y <= 0.8 &&
                leftHandJoint.transform.position.y - rootJoint.transform.position.y >= 0.5 &&
                rigthHandJoint.transform.position.y - rootJoint.transform.position.y <= 0.8 &&
                rigthHandJoint.transform.position.y - rootJoint.transform.position.y >= 0.5)
                return true;
            else
                return false;
        }
    }
}

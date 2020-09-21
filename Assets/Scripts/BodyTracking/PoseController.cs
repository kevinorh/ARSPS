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

    public double CalcularAnguloHombroCodoIzquierda(Vector3 hombro,Vector3 codo)
    {
        double angulo = 0;

        Vector3 referenciaHombroTorso = new Vector3(hombro.x, codo.y, hombro.z);

        float distanciaHombroCodo = Vector3.Distance(hombro, codo);
        float distanciaHombroReferencia = Vector3.Distance(referenciaHombroTorso, codo);
        angulo = (180/Mathf.PI) * Mathf.Asin(distanciaHombroReferencia / distanciaHombroCodo);

        if (hombro.y < codo.y && hombro.x < codo.x)
            angulo = 180 - angulo;

        else if (hombro.y < codo.y && hombro.x > codo.x)
            angulo = 180 + angulo;

        return angulo;
    }

    public double CalcularAnguloHombroCodoDerecha(Vector3 hombro,Vector3 codo)
    {
        double angulo = 0;

        Vector3 referenciaHombroTorso = new Vector3(hombro.x, codo.y, hombro.z);

        float distanciaHombroCodo = Vector3.Distance(hombro, codo);
        float distanciaHombroReferencia = Vector3.Distance(referenciaHombroTorso, codo);
        angulo = (180 / Mathf.PI) * Mathf.Asin(distanciaHombroReferencia / distanciaHombroCodo);

        if (hombro.y < codo.y && hombro.x > codo.x)
            angulo = 180 - angulo;

        else if (hombro.y < codo.y && hombro.x < codo.x)
            angulo = 180 + angulo;

        return angulo;
    }
    public double CalcularAnguloCodoManoIzquierda(Vector3 codo, Vector3 mano, double anguloHombroCodo)
    {
        double angulo = 0;

        Vector3 referenciaCodoMano = new Vector3(codo.x, mano.y, codo.z);

        float distanciaCodoMano = Vector3.Distance(codo, mano);
        float distanciaCodoReferencia = Vector3.Distance(referenciaCodoMano, mano);
        angulo = (180 / Mathf.PI) * Mathf.Asin(distanciaCodoReferencia / distanciaCodoMano);

        if (codo.y < mano.y && codo.x < mano.x)
            angulo = anguloHombroCodo + angulo;
        
        else if (codo.y < mano.y && codo.x > mano.x)
            angulo = anguloHombroCodo - angulo;

        else if (codo.y > mano.y && codo.x < mano.x)
            angulo = 180 + anguloHombroCodo - angulo;

        else if (codo.y > mano.y && codo.x > mano.x)
            angulo = 180 + anguloHombroCodo + angulo;

        return angulo;
    }
    public double CalcularAnguloCodoManoDerecha(Vector3 codo, Vector3 mano, double anguloHombroCodo)
    {
        double angulo = 0;

        Vector3 referenciaCodoMano = new Vector3(codo.x, mano.y, codo.z);

        float distanciaCodoMano = Vector3.Distance(codo, mano);
        float distanciaCodoReferencia = Vector3.Distance(referenciaCodoMano, mano);
        angulo = (180 / Mathf.PI) * Mathf.Asin(distanciaCodoReferencia / distanciaCodoMano);

        if (codo.y < mano.y && codo.x > mano.x)
            angulo = anguloHombroCodo + angulo;

        else if (codo.y < mano.y && codo.x < mano.x)
            angulo = anguloHombroCodo - angulo;

        else if (codo.y > mano.y && codo.x > mano.x)
            angulo = 180 + anguloHombroCodo - angulo;

        else if (codo.y > mano.y && codo.x < mano.x)
            angulo = 180 + anguloHombroCodo + angulo;

        return angulo;
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

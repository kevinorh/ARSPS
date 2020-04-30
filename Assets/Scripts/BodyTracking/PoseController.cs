using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PoseController : MonoBehaviour
{
    //Posicion en función a la diferencia entre el root y el joint
    public string ResumePosition(JointTracker[] joints)
    {
        JointTracker leftHandJoint = joints.Where(x => x.gameObject.transform.parent.name == "LeftHand").SingleOrDefault();
        JointTracker rigthHandJoint = joints.Where(x => x.gameObject.transform.parent.name == "RightHand").SingleOrDefault();
        JointTracker rootJoint = joints.Where(x => x.gameObject.transform.parent.name == "Root").SingleOrDefault();

        if (leftHandJoint == null || rigthHandJoint == null || rootJoint == null)
            return "false";
        else
        {
            var response = "";
            response += $"Right(x)-Root(x) (-0.3 y 0): {rigthHandJoint.transform.position.x - rootJoint.transform.position.x}\n";
            response += $"Left(x)-Root(x) (0 y 0.3): {leftHandJoint.transform.position.x - rootJoint.transform.position.x}\n";
            response += $"Right(x)-Left(x) (-0.3 y 0): {rigthHandJoint.transform.position.x - leftHandJoint.transform.position.x}\n";
            response += $"Left(y)-Right(y) (-0.1 y 0.1): {leftHandJoint.transform.position.y - rigthHandJoint.transform.position.y}\n";
            response += $"Left(y)-Root(y) (>=0.8): {leftHandJoint.transform.position.y - rootJoint.transform.position.y}\n";
            
            if (rigthHandJoint.transform.position.x - rootJoint.transform.position.x <= 0 &&
                rigthHandJoint.transform.position.x - rootJoint.transform.position.x >= -0.3 &&
                leftHandJoint.transform.position.x - rootJoint.transform.position.x >= 0 &&
                leftHandJoint.transform.position.x - rootJoint.transform.position.x <= 0.3 &&
                rigthHandJoint.transform.position.x - leftHandJoint.transform.position.x >= -0.3 &&
                rigthHandJoint.transform.position.x - leftHandJoint.transform.position.x <= 0 &&
                leftHandJoint.transform.position.y - rigthHandJoint.transform.position.y >= -0.1 &&
                leftHandJoint.transform.position.y - rigthHandJoint.transform.position.y <= 0.1 &&
                leftHandJoint.transform.position.y - rootJoint.transform.position.y >= 0.8)
                return response += "true";
            else
                return response+="false";             
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
                rigthHandJoint.transform.position.x - leftHandJoint.transform.position.x < -0.3 &&
                leftHandJoint.transform.position.y - rigthHandJoint.transform.position.y >= -0.1 &&
                leftHandJoint.transform.position.y - rigthHandJoint.transform.position.y <= 0.1 &&
                leftHandJoint.transform.position.y - rootJoint.transform.position.y >= 0.8)
                return true;
            else
                return false;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PoseController : MonoBehaviour
{
    public bool ResumePosition(JointTracker[] joints)
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
    public bool FinishPosition(JointTracker[] joints)
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

using System.Collections;
using System.Collections.Generic;
using System.Security.Policy;
using UnityEngine;

public class Mission
{

    public enum MissionType {
        Navigation,
        NavigationPerson,
        Orientation,
        OrientationPerson
    }


    public MissionType missionType;
    public Destination destination;
    public string destinationName;
    public int chosenOrientation;//The dropdown value
    public string personName;//if any..

    public Mission() {
        missionType = MissionType.Navigation;
        destination = new Destination();
        destinationName = "NULL";
        chosenOrientation = 0;
        personName = "";
    }

    public Mission(MissionType _missionType, Destination _destination, string _destinationName) {
        missionType = _missionType;
        destination = _destination;
        destinationName = _destinationName;
    }

    public void SetMissionTypeFromInt(int missionTypeInt) {
        switch (missionTypeInt) {
            case 0:
                missionType = MissionType.Navigation;
                break;
            case 1:
                missionType = MissionType.NavigationPerson;
                break;
            case 2:
                missionType = MissionType.Orientation;
                break;
            case 3:
                missionType = MissionType.OrientationPerson;
                break;
            default:
                break;
        }

    }

    public MissionType GetMissionType() {
        return missionType;
    }

    public string GetMissionTitle() {
        switch (missionType) {
            case MissionType.Navigation: return "Navigate to";
            case MissionType.NavigationPerson: return "Navigate to";
            case MissionType.OrientationPerson: return "Ortientation";
            case MissionType.Orientation: return "Ortientation";
            default:
                return "Error";
        }
    }

    public string GetDestinationName() {
        return destinationName;
    }
}

﻿// Copyright © 2018-2021 United States Government as represented by the Administrator
// of the National Aeronautics and Space Administration. All Rights Reserved.

using UnityEngine;
using GSFC.ARVR.GMSEC;
using GSFC.ARVR.MRET.XRC;
using GSFC.ARVR.MRET.Common.Schemas;

public class CollaborationManager : MonoBehaviour
{
    public static CollaborationManager instance;

    public enum EngineType { XRC, LegacyGMSEC };

    [Tooltip("The middleware type to use.")]
    public GMSECDefs.ConnectionTypes connectionType = GMSECDefs.ConnectionTypes.bolt;

    [Tooltip("The GMSEC server address and port number.")]
    public string server = "localhost:9100";

    [Tooltip("The mission name to use.")]
    public string missionName = "UNSET";

    [Tooltip("The satellite name to use.")]
    public string satName = "UNSET";

    public SynchronizationManager synchManager;
    public SessionMasterNode masterNode;
    public SessionSlaveNode slaveNode;
    public UnityProject projectManager;
    public ModeNavigator modeNavigator;
    public EngineType engineType = EngineType.XRC;
    public MonoGMSEC gmsec;
    public XRCManager xrcManager;
    public GameObject vrAvatarPrefab, desktopAvatarPrefab;
    public GameObject userContainer;

    private SessionAdvertiser sessionAdvertiser;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        if (gmsec == null)
        {
            gmsec = FindObjectOfType<MonoGMSEC>();
        }

        if (xrcManager == null)
        {
            xrcManager = FindObjectOfType<XRCManager>();
        }
    }

    public void EnterMasterMode(SessionInformation sessionInfo, string alias)
    {
#if !HOLOLENS_BUILD
        if (engineType == EngineType.LegacyGMSEC)
        {
            synchManager.enabled = true;
            slaveNode.enabled = false;
            masterNode.enabled = true;

            masterNode.StartRunning(sessionInfo, alias);

            sessionAdvertiser = gameObject.AddComponent<SessionAdvertiser>();
            synchManager.sessionAdvertiser = sessionAdvertiser;
            synchManager.masterNode = masterNode;
            sessionAdvertiser.sessionInformation = sessionInfo;
            sessionAdvertiser.connectionType = connectionType;
            sessionAdvertiser.server = server;
            sessionAdvertiser.missionName = missionName;
            sessionAdvertiser.satName = satName;
            sessionAdvertiser.Initialize();

            modeNavigator.projectManager.userAlias = alias;
            modeNavigator.OpenProject(sessionInfo.projectName, true);
        }
#endif
    }

    public void EnterSlaveMode(SessionInformation sessionInfo, string alias)
    {
        if (engineType == EngineType.LegacyGMSEC)
        {
            Destroy(sessionAdvertiser);

            synchManager.enabled = true;
            masterNode.enabled = false;
            slaveNode.enabled = true;

            slaveNode.Connect(sessionInfo, alias);

            modeNavigator.projectManager.userAlias = alias;
            modeNavigator.OpenProject(Application.dataPath + "/Projects/" + sessionInfo.projectName, true);
        }
    }
}
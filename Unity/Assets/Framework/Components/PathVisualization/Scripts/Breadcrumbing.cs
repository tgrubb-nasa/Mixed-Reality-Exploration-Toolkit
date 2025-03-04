﻿// Copyright © 2018-2021 United States Government as represented by the Administrator
// of the National Aeronautics and Space Administration. All Rights Reserved.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Breadcrumbing : MonoBehaviour
{
    [Tooltip("Objet to be breadcrumbed")]
    public GameObject breadcrumbingObject;
    public ArrayList objPosition = new ArrayList();
    
    // add positions to an array list
    void Update()
    {
        objPosition.Add(breadcrumbingObject.transform.position);
    }
}

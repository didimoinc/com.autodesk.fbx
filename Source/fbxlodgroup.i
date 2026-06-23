// ***********************************************************************
// Copyright (c) 2017 Unity Technologies. All rights reserved.
//
// Licensed under the ##LICENSENAME##.
// See LICENSE.md file in the project root for full license information.
// ***********************************************************************

// Unignore class
%rename("%s", %$isclass) FbxLODGroup;

// EDisplayLevel enum
%rename("%s") FbxLODGroup::EDisplayLevel;

// Properties (mark immutable to reveal; .Get()/.Set() still work on the FbxPropertyBool/Double objects)
%fbximmutable(FbxLODGroup::ThresholdsUsedAsPercentage);
%fbximmutable(FbxLODGroup::MinMaxDistance);
%fbximmutable(FbxLODGroup::MinDistance);
%fbximmutable(FbxLODGroup::MaxDistance);
%fbximmutable(FbxLODGroup::WorldSpace);

// Methods
%rename("%s") FbxLODGroup::GetNumThresholds;
%rename("%s") FbxLODGroup::AddThreshold(FbxDouble pThreshValue);
%rename("%s") FbxLODGroup::GetNumDisplayLevels;
%rename("%s") FbxLODGroup::AddDisplayLevel;
%rename("%s") FbxLODGroup::SetDisplayLevel;

// Ignore overloads that use FbxDistance (not bound) and out-parameter variants
%ignore FbxLODGroup::AddThreshold(const FbxDistance&);
%ignore FbxLODGroup::SetThreshold;
%ignore FbxLODGroup::GetThreshold;
%ignore FbxLODGroup::GetDisplayLevel;

%include "fbxsdk/scene/geometry/fbxlodgroup.h"

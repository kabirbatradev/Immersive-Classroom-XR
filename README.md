# immersive-vis-in-large-lecture

**Immersive Visualization in Large Lecture (IVLL)** is a project for an in-progress research paper. The goal is to deploy mixed reality (MR) devices in a large lecture environment. This will alow students to view immersive 3D visualizations to enhance their learning, all the while seeing elements of the real world to avoid cybersickness.

Features:
- Mixed reality passthrough (you can see the real world)
- Shared coordinate space (users see virtual objects in the same physical space)
- Custom interactable virtual objects
- Student and object group partitioning: a student in group 1 cannot see or interact with objects in group 2
- Group assigning: admin can put individual students into different groups
- Basic automatic group partitioning algorithms: students can be assigned to their own individual group, random pairs, or the whole class is in one group
- Admin/Cameraman mode: cameraman can see all objects regardless of group number

In Progress:
- Advanced automatic group partitioning algorithm: put students into groups of a specified size automatically
- 2D classroom visualization: view the classroom from a bird's eye view on a computer screen
- Switch between breakout groups and large lecture: In breakout groups mode, every group has their own virtual interactable objects. In large lecture mode, one large uninteractable object is present at the front of the class.

## Note

This repository builds off of the Unity-SharedSpatialAnchors sample by Meta.

Sample App Architecture: https://developer.oculus.com/documentation/unity/unity-ssa-sf/

Github Repository: https://github.com/oculus-samples/Unity-SharedSpatialAnchors

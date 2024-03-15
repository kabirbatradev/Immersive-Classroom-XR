# immersive-visualization-in-large-lecture

**Immersive Visualization in Large Lecture (IVLL)** is a project for an in-progress research paper. The goal is to deploy mixed reality (MR) devices in a large lecture environment. This will alow students to view immersive 3D visualizations to enhance their learning, all the while seeing elements of the real world to avoid cybersickness.

### HMD (Head Mounted Display) Features:
- Mixed reality passthrough (you can see the real world).
- Shared coordinate space (users see virtual objects in the same physical space).
- Custom interactable virtual objects.
- Student and object group partitioning: A student in group 1 cannot see or interact with objects in group 2.
- Virtual Aligned Desks: Admin can create virtual desks aligned with the classroom's real desks to estimate the seat a student is sitting at; used for grouping.
- Automatic group partitioning algorithms: Students can be assigned to their own individual group, groups of 4, group by desk row, or the whole class is in one group.
- Switch between breakout groups and large lecture: In breakout groups mode, every group has their own virtual interactable objects. In large lecture mode, one large uninteractable object is present at the front of the class.
- Cameraman mode: Cameraman can see all objects regardless of group number.
- Theater Mode: The ceiling opens up and the walls fall, revealing 360 video and images.

### Instructor GUI Display Features:
- Presentation Object Control: Instantiate, cycle between, and rotate 3D objects currently being displayed to the classroom.
- Laser Pointer: Instructor can use their mouse to point at a spot on the virtual object, and each group of students will see an instantiated laser beam for their instance of the presentation object.
- 2D classroom visualization: View the classroom from a bird's eye view on a computer screen.
- Theater Mode Control and visualization: Trigger theater mode on devices, view how much the walls have dropped, and control what 360 media is displayed.
- Stabilized View: Instructor can see a stabilized view of a student's perspective.


### Note:

This repository builds off of (and heavily modifies) the Unity-SharedSpatialAnchors sample by Meta.

Sample App Architecture: https://developer.oculus.com/documentation/unity/unity-ssa-sf/

Github Repository: https://github.com/oculus-samples/Unity-SharedSpatialAnchors

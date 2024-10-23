# immersive-visualization-in-large-lecture

**Immersive Visualization in Large Lecture (IVLL)** is a project for a research paper and user study. The goal is to deploy mixed reality (MR) devices in a large lecture environment. This will alow students to view immersive 3D visualizations to enhance their learning, all the while seeing the real world to avoid cybersickness.

### HMD (Head Mounted Display) Features:
- Shared coordinate space: Students see virtual objects in the same physical space.
- Custom interactable virtual objects controlled by instructor.
- 360 Theater: The ceiling opens up and the walls fall, revealing 360 video and images.
- Student and object group partitioning: A student in group 1 cannot see or interact with objects in group 2.
- Virtual Aligned Desks: Admin can create virtual desks aligned with the classroom's real desks to estimate the seat a student is sitting at; used for grouping.
- Automatic group partitioning algorithms: Students can be assigned to their own individual group, groups of 4, group by desk row, or the whole class is in one group.
- Switch between breakout groups and large lecture: In breakout groups, every group has their own virtual interactable objects. In large lecture mode, one large uninteractable object is present at the front of the class.
- Quiz Side Panel: Instructor can instantiate an interactable floating panel placed next to each group containing an quiz questions. 
- Instructor help: When students request for help, a floating panel showing the instructor’s face appears (laptop camera feed), allowing the instructor to easily converse with students across the room.
- Camera device: The camera headset can see all objects regardless of group number.

### Additional Instructor GUI Display Features:
- Presentation Object Control: Instantiate, cycle between, and rotate 3D objects currently being displayed to the classroom.
- Laser Pointer: Instructor can use their mouse to point at a spot on the virtual object, and each group of students will see an instantiated laser beam for their instance of the presentation object.
- 2D classroom visualization: View the classroom from a bird's eye view on a computer screen.
- Theater Mode Control and visualization: Trigger theater mode on devices, view how much the walls have dropped, and control what 360 media is displayed.
- Stabilized View: Instructor can see a stabilized view of a student's perspective.


### Note:

This repository uses the Unity-SharedSpatialAnchors sample by Meta as a starting point.

Sample App Architecture: https://developer.oculus.com/documentation/unity/unity-ssa-sf/

Github Repository: https://github.com/oculus-samples/Unity-SharedSpatialAnchors

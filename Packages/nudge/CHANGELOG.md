# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]
- Window to show more info per line
- Custom icon for commentSO in project (unity atoms has a solve for this https://github.com/AdamRamberg/unity-atoms/commit/cb6280a2554c9ac536d17485865fc4ae4ea85138)
- Add Comment changed event to smooth editor window refresh logic

## [0.1.0] - 
- Comment class with property drawer, CommentBeh (component), and CommentSO asset
- Comments can be hidden
- Comments can be marked as tasks, indicating they require action
- Task and Comment window to locate and sort all comments in the project and currently open scenes.
- Nudge Settings to customise colour tints.
- Comments have gizmo icons in scene view, if referencing a scene object it also can have an icon
# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]


## [0.3.0] - 2020-09-24
### Added
 - Comments can now reference a list of additional UnityEngine.Objects
 - Shortcuts ctrl shift c and ctrl alt shift c to create Scene Comment and Asset Comment, respectively.
   - This is now the primary way to create comments.
   - Comments created in this manner attempt to auto populate their linked object(s) from you current selection.


## [0.2.0] - 2020-07-29
### Added
 - Nudge Logo to site's README.md.

### Changed
 - Comment Body uses WordWrap, simplify property drawer.
 - README.md includes a logo and basic instructions for installing system git.

## [0.1.0] - 2020-07-07
### Added
- Comment class with property drawer, CommentBeh (component), and CommentSO asset.
- Comments can be hidden.
- Comments can be marked as tasks, indicating they require action.
- Task and Comment window to locate and sort all comments in the project and currently open scenes.
- Nudge Settings to customise colour tints.
- Comments have gizmo icons in scene view, if referencing a scene object it also can have an icon.
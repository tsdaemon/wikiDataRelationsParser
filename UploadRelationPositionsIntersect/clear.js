﻿db.triplet.update({ ArticlePositions: { $ne: null } }, { $set: { ArticlePositions: null } }, { multi: true })

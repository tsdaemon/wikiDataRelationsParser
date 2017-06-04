db.triplet.update({ ArticlePositions: { $ne: null } }, { $unset: { ArticlePositions: "" } }, { multi: true })

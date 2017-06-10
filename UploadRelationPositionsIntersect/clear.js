db.triplet.update({}, { $unset: { ArticlePositions: "" } }, { multi: true })

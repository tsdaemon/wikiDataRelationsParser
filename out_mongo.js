db.triplet.find({SectionId:"organization"}).forEach(function(t) {
    print(t.WikiResult.object + "\t" + t.WikiResult.objectLabel + "\t" + t.WikiResult.objectWiki + "\t"
    + t.Property + "\t" 
    + t.WikiResult.subject + "\t" + t.WikiResult.subjectLabel + "\t" + t.WikiResult.subjectWiki);
})
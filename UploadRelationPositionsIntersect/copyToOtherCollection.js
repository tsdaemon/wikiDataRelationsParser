﻿db.triplet_train.aggregate([{$match:{PredicateId:"P159"}}, {$out:"triplet_train_P159"}])
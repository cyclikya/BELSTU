// queries.js
// БД: license_management
// Коллекции: licenses, usage_places

use("license_management");

// 1. Изменение и обновление элементов

db.licenses.updateOne(
  { name: "AutoCAD" },
  { $set: { price: 700, active: true } }
);

db.licenses.updateMany(
  { type: "subscription" },
  { $set: { paymentModel: "yearly" } }
);

db.usage_places.updateOne(
  { computer: "PC-310" },
  { $set: { installed: true } }
);

// 2. Условные операции

db.licenses.find({ price: { $gt: 200 } });

db.licenses.find({ price: { $gte: 120, $lte: 650 } });

db.licenses.find({ active: true });

// 3. Операторы работы с массивами

db.licenses.find({ users: "admin" });

db.licenses.find({ users: { $all: ["developer", "admin"] } });

db.usage_places.find({ tags: { $in: ["server", "finance"] } });

// 4. $exists

db.licenses.find({ paymentModel: { $exists: true } });

// 5. $type

db.licenses.find({ price: { $type: "number" } });

db.licenses.find({ name: { $type: "string" } });

// 6. $regex

db.licenses.find({ name: { $regex: "Microsoft", $options: "i" } });

db.usage_places.find({ responsible: { $regex: "^S", $options: "i" } });

// 7. Проекции

db.licenses.find(
  {},
  { _id: 0, name: 1, vendor: 1, price: 1 }
);

db.usage_places.find(
  { installed: true },
  { _id: 0, licenseName: 1, department: 1, computer: 1 }
);

// 8. count()

db.licenses.countDocuments();

db.licenses.countDocuments({ active: true });

db.usage_places.countDocuments({ department: "IT" });

// 9. limit() и skip()

db.licenses.find().limit(3);

db.licenses.find().skip(2).limit(2);

// 10. distinct()

db.licenses.distinct("vendor");

db.licenses.distinct("type");

db.usage_places.distinct("department");

// 11. aggregate(), пустой match

db.licenses.aggregate([
  { $match: {} },
  {
    $group: {
      _id: "$vendor",
      totalLicenses: { $sum: 1 },
      averagePrice: { $avg: "$price" }
    }
  }
]);

// 12. aggregate(), непустой match

db.licenses.aggregate([
  { $match: { active: true } },
  {
    $group: {
      _id: "$type",
      count: { $sum: 1 },
      maxPrice: { $max: "$price" },
      minPrice: { $min: "$price" }
    }
  }
]);

// 13. Группировка по нескольким ключам

db.licenses.aggregate([
  {
    $group: {
      _id: {
        vendor: "$vendor",
        type: "$type"
      },
      count: { $sum: 1 },
      totalPrice: { $sum: "$price" }
    }
  }
]);

// 14. aggregate() с сортировкой и project

db.licenses.aggregate([
  { $match: { price: { $gt: 100 } } },
  { $sort: { price: -1 } },
  {
    $project: {
      _id: 0,
      name: 1,
      vendor: 1,
      price: 1
    }
  }
]);
// lab14_queries.js
// Вариант 12: Управление лицензиями на ПО
// Запуск в MongoDB Shell: load("lab14_queries.js")

use("license_management");

// очистка
db.licenses.drop();
db.usage_places.drop();

// добавление данных
db.licenses.insertMany([
  {
    name: "Microsoft Office 365",
    type: "subscription",
    vendor: "Microsoft",
    price: 120,
    active: true,
    users: ["admin", "manager", "accountant"],
    expireDate: new Date("2026-12-31")
  },
  {
    name: "Adobe Photoshop",
    type: "subscription",
    vendor: "Adobe",
    price: 240,
    active: true,
    users: ["designer", "manager"],
    expireDate: new Date("2026-08-15")
  },
  {
    name: "Windows Server 2022",
    type: "permanent",
    vendor: "Microsoft",
    price: 900,
    active: true,
    users: ["admin"],
    expireDate: null
  },
  {
    name: "AutoCAD",
    type: "subscription",
    vendor: "Autodesk",
    price: 650,
    active: false,
    users: ["engineer"],
    expireDate: new Date("2025-11-20")
  },
  {
    name: "IntelliJ IDEA",
    type: "subscription",
    vendor: "JetBrains",
    price: 170,
    active: true,
    users: ["developer", "admin"],
    expireDate: new Date("2026-06-01")
  }
]);

db.usage_places.insertMany([
  {
    licenseName: "Microsoft Office 365",
    department: "Accounting",
    computer: "PC-101",
    responsible: "Ivanov",
    installed: true,
    tags: ["office", "finance"]
  },
  {
    licenseName: "Adobe Photoshop",
    department: "Design",
    computer: "PC-205",
    responsible: "Petrova",
    installed: true,
    tags: ["design", "graphics"]
  },
  {
    licenseName: "Windows Server 2022",
    department: "IT",
    computer: "SRV-01",
    responsible: "Sidorov",
    installed: true,
    tags: ["server", "system"]
  },
  {
    licenseName: "AutoCAD",
    department: "Engineering",
    computer: "PC-310",
    responsible: "Smirnov",
    installed: false,
    tags: ["engineering"]
  },
  {
    licenseName: "IntelliJ IDEA",
    department: "Development",
    computer: "PC-404",
    responsible: "Kozlov",
    installed: true,
    tags: ["development", "java"]
  }
]);

// изменение и обновление элементов
db.licenses.updateOne(
  { name: "AutoCAD" },
  { $set: { active: true, price: 700 } }
);

db.licenses.updateMany(
  { type: "subscription" },
  { $set: { paymentModel: "yearly" } }
);

db.usage_places.updateOne(
  { computer: "PC-310" },
  { $set: { installed: true } }
);

// условные операции
db.licenses.find({ price: { $gt: 200 } });
db.licenses.find({ price: { $gte: 120, $lte: 650 } });
db.licenses.find({ active: true });

// операторы работы с массивами
db.licenses.find({ users: "admin" });
db.licenses.find({ users: { $all: ["admin", "developer"] } });
db.usage_places.find({ tags: { $in: ["server", "finance"] } });

// $exists
db.licenses.find({ paymentModel: { $exists: true } });

// $type
db.licenses.find({ price: { $type: "number" } });
db.licenses.find({ name: { $type: "string" } });

// $regex
db.licenses.find({ name: { $regex: "Microsoft", $options: "i" } });
db.usage_places.find({ responsible: { $regex: "^S", $options: "i" } });

// проекции
db.licenses.find(
  {},
  { name: 1, vendor: 1, price: 1, _id: 0 }
);

db.usage_places.find(
  { installed: true },
  { licenseName: 1, department: 1, computer: 1, _id: 0 }
);

// count()
db.licenses.countDocuments();
db.licenses.countDocuments({ active: true });
db.usage_places.countDocuments({ department: "IT" });

// limit() и skip()
db.licenses.find().limit(3);
db.licenses.find().skip(2).limit(2);

// distinct()
db.licenses.distinct("vendor");
db.licenses.distinct("type");
db.usage_places.distinct("department");

// aggregate(), пустой match
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

// aggregate(), непустой match
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

// группировка по нескольким ключам
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

// сортировка внутри aggregate
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
CREATE TABLE Students
(
    StudentId NUMBER PRIMARY KEY,
    FullName NVARCHAR2(100) NOT NULL,
    GroupName NVARCHAR2(20) NOT NULL,
    AverageMark NUMBER(4,2),
    AdmissionDate DATE
);


SELECT * FROM Students;

Drop TABLE Students;



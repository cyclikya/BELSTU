"""Читает отчёт покрытия cobertura и печатает сводку по классам."""
import glob
import sys
from xml.etree import ElementTree as ET

files = glob.glob("TestResults/**/coverage.cobertura.xml", recursive=True)
if not files:
    sys.exit("отчёт покрытия не найден: сначала выполните dotnet test --collect:\"XPlat Code Coverage\"")

root = ET.parse(max(files)).getroot()
print(f"{'Класс':<34}{'Строки':>10}{'Покрыто':>10}{'%':>8}{'Ветви %':>10}")
print("-" * 72)

total_lines = covered_lines = 0
for cls in sorted(root.iter("class"), key=lambda c: c.get("name")):
    lines = cls.findall(".//line")
    if not lines:
        continue
    hit = sum(1 for ln in lines if int(ln.get("hits", 0)) > 0)
    total_lines += len(lines)
    covered_lines += hit
    name = cls.get("name", "").split(".")[-1]
    branch = float(cls.get("branch-rate", 0)) * 100
    print(f"{name:<34}{len(lines):>10}{hit:>10}{hit / len(lines) * 100:>7.1f}%{branch:>9.1f}%")

print("-" * 72)
line_rate = float(root.get("line-rate", 0)) * 100
branch_rate = float(root.get("branch-rate", 0)) * 100
print(f"{'ИТОГО':<34}{total_lines:>10}{covered_lines:>10}{line_rate:>7.1f}%{branch_rate:>9.1f}%")

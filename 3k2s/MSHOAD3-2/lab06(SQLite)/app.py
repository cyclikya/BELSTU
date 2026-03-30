import sqlite3
from datetime import datetime

class SoftwareLicenseManager:
    def __init__(self, db_name='DB.db'):
        self.conn = sqlite3.connect(db_name)
        self.conn.row_factory = sqlite3.Row
        self.cursor = self.conn.cursor()
        self.cursor.execute("PRAGMA foreign_keys = ON")
    
    def close(self):
        self.conn.close()
    
    def show_license_usage(self):
        print("\n" + "="*80)
        print("LICENSE USAGE VIEW (vw_license_usage)")
        print("="*80)
        
        self.cursor.execute("SELECT * FROM vw_license_usage LIMIT 10")
        rows = self.cursor.fetchall()
        
        if rows:
            print(f"{'ID':<6} {'SOFTWARE':<20} {'KEY':<15} {'TOTAL':<8} {'USED':<12} {'FREE':<8}")
            print("-"*80)
            for row in rows:
                print(f"{row['license_id']:<6} {row['software_name']:<20} "
                      f"{row['license_key']:<15} {row['total_seats']:<8} "
                      f"{row['used_seats']:<12} {row['free_seats']:<8}")
        else:
            print("No data found")
    
    def show_employee_licenses(self):
        print("\n" + "="*80)
        print("EMPLOYEE LICENSES")
        print("="*80)
        
        self.cursor.execute("""
            SELECT e.employee_id, e.full_name, s.name as software_name, 
                   la.assigned_date, la.status, la.device_name
            FROM LicenseAssignments la
            JOIN Employees e ON la.employee_id = e.employee_id
            JOIN Licenses l ON la.license_id = l.license_id
            JOIN Software s ON l.software_id = s.software_id
            LIMIT 10
        """)
        
        rows = self.cursor.fetchall()
        
        if rows:
            print(f"{'ID':<6} {'EMPLOYEE':<25} {'SOFTWARE':<20} {'DATE':<12} {'STATUS':<10} {'DEVICE':<20}")
            print("-"*80)
            for row in rows:
                print(f"{row['employee_id']:<6} {row['full_name']:<25} "
                      f"{row['software_name']:<20} {row['assigned_date']:<12} "
                      f"{row['status']:<10} {row['device_name']:<20}")
        else:
            print("No assignments found")
    
    def add_license_assignment(self, license_id, employee_id, device_type, device_name):
        print("\n" + "="*80)
        print("ADDING LICENSE ASSIGNMENT (Trigger Demonstration)")
        print("="*80)
        
        try:
            self.cursor.execute("BEGIN TRANSACTION")
            
            self.cursor.execute("SELECT license_id, used_seats, total_seats FROM Licenses WHERE license_id = ?", 
                               (license_id,))
            license = self.cursor.fetchone()
            
            if not license:
                print(f"Error: License ID {license_id} not found")
                self.cursor.execute("ROLLBACK")
                return False
            
            if license['used_seats'] >= license['total_seats']:
                print(f"Error: No available seats for license {license_id}")
                self.cursor.execute("ROLLBACK")
                return False
            
            self.cursor.execute("SELECT employee_id, full_name FROM Employees WHERE employee_id = ?", 
                               (employee_id,))
            employee = self.cursor.fetchone()
            
            if not employee:
                print(f"Error: Employee ID {employee_id} not found")
                self.cursor.execute("ROLLBACK")
                return False
            
            assigned_date = datetime.now().strftime('%Y-%m-%d')
            self.cursor.execute("""
                INSERT INTO LicenseAssignments 
                (license_id, employee_id, assigned_date, status, device_type, device_name)
                VALUES (?, ?, ?, 'Active', ?, ?)
            """, (license_id, employee_id, assigned_date, device_type, device_name))
            
            self.cursor.execute("COMMIT")
            
            print(f"Success: License assigned to {employee['full_name']}")
            print(f"  Assignment date: {assigned_date}")
            
            self.cursor.execute("SELECT used_seats, total_seats FROM Licenses WHERE license_id = ?", 
                               (license_id,))
            updated = self.cursor.fetchone()
            print(f"  Used seats: {updated['used_seats']}/{updated['total_seats']}")
            
            return True
            
        except sqlite3.Error as e:
            print(f"Database error: {e}")
            self.cursor.execute("ROLLBACK")
            return False
    
    def revoke_license(self, assignment_id):
        print("\n" + "="*80)
        print("REVOKING LICENSE")
        print("="*80)
        
        try:
            self.cursor.execute("""
                SELECT la.assignment_id, la.license_id, e.full_name, l.used_seats
                FROM LicenseAssignments la
                JOIN Employees e ON la.employee_id = e.employee_id
                JOIN Licenses l ON la.license_id = l.license_id
                WHERE la.assignment_id = ?
            """, (assignment_id,))
            
            assignment = self.cursor.fetchone()
            
            if not assignment:
                print(f"Error: Assignment ID {assignment_id} not found")
                return False
            
            print(f"Assignment found:")
            print(f"  Employee: {assignment['full_name']}")
            print(f"  License ID: {assignment['license_id']}")
            print(f"  Current usage: {assignment['used_seats']}")
            
            self.cursor.execute("BEGIN TRANSACTION")
            self.cursor.execute("DELETE FROM LicenseAssignments WHERE assignment_id = ?", 
                               (assignment_id,))
            self.cursor.execute("COMMIT")
            
            print(f"Success: License revoked from {assignment['full_name']}")
            
            self.cursor.execute("SELECT used_seats, total_seats FROM Licenses WHERE license_id = ?", 
                               (assignment['license_id'],))
            updated = self.cursor.fetchone()
            print(f"  Updated usage: {updated['used_seats']}/{updated['total_seats']}")
            
            return True
            
        except sqlite3.Error as e:
            print(f"Database error: {e}")
            self.cursor.execute("ROLLBACK")
            return False
    
    def update_assignment_status(self, assignment_id, new_status):
        print("\n" + "="*80)
        print("UPDATING ASSIGNMENT STATUS")
        print("="*80)
        
        try:
            self.cursor.execute("""
                SELECT assignment_id, status FROM LicenseAssignments 
                WHERE assignment_id = ?
            """, (assignment_id,))
            
            assignment = self.cursor.fetchone()
            if not assignment:
                print(f"Error: Assignment ID {assignment_id} not found")
                return False
            
            print(f"Current status: {assignment['status']}")
            
            self.cursor.execute("BEGIN TRANSACTION")
            self.cursor.execute("""
                UPDATE LicenseAssignments 
                SET status = ? 
                WHERE assignment_id = ?
            """, (new_status, assignment_id))
            self.cursor.execute("COMMIT")
            
            print(f"Status updated: {assignment['status']} -> {new_status}")
            return True
            
        except sqlite3.Error as e:
            print(f"Database error: {e}")
            self.cursor.execute("ROLLBACK")
            return False
    
    def test_foreign_key_constraints(self):
        print("\n" + "="*80)
        print("TESTING FOREIGN KEY CONSTRAINTS")
        print("="*80)
        
        print("\nAttempting to insert with non-existent license (license_id=999):")
        try:
            self.cursor.execute("BEGIN TRANSACTION")
            self.cursor.execute("""
                INSERT INTO LicenseAssignments
                (license_id, employee_id, assigned_date, status, device_type, device_name)
                VALUES (999, 1, '2025-06-01', 'Active', 'Laptop', 'Test Device')
            """)
            self.cursor.execute("COMMIT")
            print("  Unexpected: Insert succeeded")
        except sqlite3.Error as e:
            print(f"  Expected error: {e}")
            self.cursor.execute("ROLLBACK")
        
        print("\nAttempting to update license_id to non-existent value:")
        try:
            self.cursor.execute("BEGIN TRANSACTION")
            self.cursor.execute("UPDATE LicenseAssignments SET license_id = 999 WHERE assignment_id = 2")
            self.cursor.execute("COMMIT")
            print("  Unexpected: Update succeeded")
        except sqlite3.Error as e:
            print(f"  Expected error: {e}")
            self.cursor.execute("ROLLBACK")
    
    def show_index_usage(self):
        print("\n" + "="*80)
        print("INDEX USAGE DEMONSTRATION")
        print("="*80)
        
        print("\nQuery plan for: SELECT * FROM LicenseAssignments WHERE license_id = 2")
        self.cursor.execute("EXPLAIN QUERY PLAN SELECT * FROM LicenseAssignments WHERE license_id = 2")
        plan = self.cursor.fetchall()
        for row in plan:
            print(f"  {row[3]}")
        
        print("\nQuery plan for: SELECT * FROM LicenseAssignments WHERE status = 'Active'")
        self.cursor.execute("EXPLAIN QUERY PLAN SELECT * FROM LicenseAssignments WHERE status = 'Active'")
        plan = self.cursor.fetchall()
        for row in plan:
            print(f"  {row[3]}")

def main():
    app = SoftwareLicenseManager('DB.db')
    
    try:
        print("\n" + "="*80)
        print("SOFTWARE LICENSE MANAGEMENT SYSTEM")
        print("="*80)
        
        app.show_license_usage()
        app.show_employee_licenses()
        
        print("\n" + "="*80)
        print("TRIGGER DEMONSTRATION")
        print("="*80)
        print("Trigger trg_after_insert_assignment automatically increments used_seats")
        
        app.add_license_assignment(
            license_id=4,
            employee_id=3,
            device_type="Laptop",
            device_name="Trigger Test Laptop"
        )
        
        app.update_assignment_status(assignment_id=1, new_status="Expired")
        
        app.test_foreign_key_constraints()
        
        app.show_index_usage()
        
        print("\n" + "="*80)
        print("Demonstration completed")
        print("="*80)
        
    finally:
        app.close()

if __name__ == "__main__":
    main()
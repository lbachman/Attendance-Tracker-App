-- Drop tables if they exist

DROP TABLE IF EXISTS days;
DROP TABLE IF EXISTS student_class;
DROP TABLE IF EXISTS attends;
DROP TABLE IF EXISTS class;
DROP TABLE IF EXISTS students;
DROP TABLE IF EXISTS users;
DROP TABLE IF EXISTS communication;
DROP TABLE IF EXISTS message;


-- Create users table without type
CREATE TABLE users (
    user_id INT UNSIGNED NOT NULL AUTO_INCREMENT PRIMARY KEY,
    userName VARCHAR(40) NOT NULL
) ENGINE=InnoDB;

-- Create days table
CREATE TABLE days (
    class_id INT UNSIGNED NOT NULL,
    day VARCHAR(10) NOT NULL
) ENGINE=InnoDB;

-- Create class table
CREATE TABLE class (
    class_id INT UNSIGNED NOT NULL AUTO_INCREMENT PRIMARY KEY,
    semester_code VARCHAR(10) NOT NULL,
    room VARCHAR(10) NOT NULL,
    start_time TIME NOT NULL,
    end_time TIME NOT NULL,
    days VARCHAR(10) NOT NULL,
    instructor_id INT UNSIGNED
) ENGINE=InnoDB;

-- Create students table
CREATE TABLE students (
    student_uuid VARCHAR(64) NOT NULL PRIMARY KEY,
    student_userName VARCHAR(15) NOT NULL
) ENGINE=InnoDB;

-- Create student_class table
CREATE TABLE student_class (
    student_uuid VARCHAR(64) NOT NULL,
    class_id INT UNSIGNED NOT NULL,
    PRIMARY KEY (student_uuid, class_id)
) ENGINE=InnoDB;

-- Create attends table
CREATE TABLE attends (
    student_uuid VARCHAR(64) NOT NULL,
    class_id INT UNSIGNED NOT NULL,
    attendance_date DATETIME NOT NULL,
    PRIMARY KEY (student_uuid, class_id, attendance_date)
    ) ENGINE=InnoDB;

-- Create message table
CREATE TABLE message (
	message_id INT UNSIGNED NOT NULL AUTO_INCREMENT PRIMARY KEY,
    message TEXT(1024) NOT NULL
	) ENGINE=InnoDB;

-- Create communication table 
CREATE TABLE communication (
	com_id INT UNSIGNED NOT NULL AUTO_INCREMENT PRIMARY KEY,
    instructor_id INT UNSIGNED NOT NULL,
    student_uuid VARCHAR(64) NOT NULL,
    class_id INT UNSIGNED NOT NULL,
    message_id INT UNSIGNED NOT NULL
    ) ENGINE=InnoDB;


ALTER TABLE days
	ADD FOREIGN KEY (class_id) REFERENCES class(class_id) ON DELETE CASCADE ON UPDATE CASCADE;
ALTER TABLE student_class 
	ADD FOREIGN KEY (student_uuid) REFERENCES students(student_uuid) ON DELETE CASCADE ON UPDATE CASCADE,
    ADD FOREIGN KEY (class_id) REFERENCES class(class_id) ON DELETE CASCADE ON UPDATE CASCADE;
ALTER TABLE class
	ADD FOREIGN KEY (instructor_id) REFERENCES users(user_id) ON DELETE SET NULL ON UPDATE CASCADE;
ALTER TABLE attends 
	ADD FOREIGN KEY (student_uuid) REFERENCES students(student_uuid) ON DELETE CASCADE ON UPDATE CASCADE,
    	ADD FOREIGN KEY (class_id) REFERENCES class(class_id) ON DELETE CASCADE ON UPDATE CASCADE;
ALTER TABLE class
	ADD COLUMN is_active BOOLEAN NOT NULL DEFAULT TRUE;
ALTER TABLE communication
	ADD FOREIGN KEY (message_id) REFERENCES message(message_id) ON DELETE CASCADE ON UPDATE CASCADE;

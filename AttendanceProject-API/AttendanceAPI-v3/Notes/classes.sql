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


-- instructors
INSERT INTO users (userName) VALUES
('john_doe'),
('alice_smith'),
('bob_jones'),
('emily_davis'),
('michael_wilson');

-- 5 instructors total teaching 10 classes
INSERT INTO class (semester_code, room, start_time, end_time, days, instructor_id) VALUES
('202401', 'A101', '08:00:00', '09:30:00', 'MWF', 1), -- Math 101 - Introduction to Calculus
('202402', 'B204', '10:00:00', '11:30:00', 'TTH', 2), -- ENG 202 - Shakespearean Literature
('202403', 'C305', '13:00:00', '14:30:00', 'MWF', 3), -- CS 301 - Data Structures and Algorithms
('202404', 'D102', '09:00:00', '10:30:00', 'TTH', 4), -- BIO 103 - Anatomy and Physiology
('202405', 'E203', '11:00:00', '12:30:00', 'MWF', 5), -- ART 205 - Digital Art and Design
('202406', 'F306', '13:30:00', '15:00:00', 'TTH', 1), -- HIS 401 - American History: Civil War Era
('202407', 'G104', '08:30:00', '10:00:00', 'MWF', 2), -- PSY 301 - Abnormal Psychology
('202408', 'H205', '10:30:00', '12:00:00', 'TTH', 3), -- BUS 202 - Marketing Principles
('202409', 'I303', '14:00:00', '15:30:00', 'MWF', 4), -- CHEM 201 - Organic Chemistry
('202410', 'J106', '08:00:00', '09:30:00', 'TTH', 5); -- MUS 101 - Music Theory


INSERT INTO days (class_id, day) VALUES
(1, 'Monday'),
(1, 'Wednesday'),
(1, 'Friday'),
(2, 'Tuesday'),
(2, 'Thursday'),
(3, 'Monday'),
(3, 'Wednesday'),
(3, 'Friday'),
(4, 'Tuesday'),
(4, 'Thursday'),
(5, 'Monday'),
(5, 'Wednesday'),
(5, 'Friday'),
(6, 'Tuesday'),
(6, 'Thursday'),
(7, 'Monday'),
(7, 'Wednesday'),
(7, 'Friday'),
(8, 'Tuesday'),
(8, 'Thursday'),
(9, 'Monday'),
(9, 'Wednesday'),
(9, 'Friday'),
(10, 'Tuesday'),
(10, 'Thursday');


INSERT INTO students (student_uuid, student_userName)
VALUES
    ('uuid1', 'jrowling'),
    ('uuid2', 'sking'),
    ('uuid3', 'jtolkien'),
    ('uuid4', 'gorwell'),
    ('uuid5', 'hlee'),
    ('uuid6', 'ehemingway'),
    ('uuid7', 'achristie'),
    ('uuid8', 'jausten'),
    ('uuid9', 'mtwain'),
    ('uuid10', 'ltolstoy'),
    ('uuid11', 'fdostoevsky'),
    ('uuid12', 'vwoolf'),
    ('uuid13', 'ggarciamarquez'),
    ('uuid14', 'cdickens'),
    ('uuid15', 'wshakespeare');


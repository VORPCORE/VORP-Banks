CREATE TABLE `banks` (
	`name` VARCHAR(50) NOT NULL,
	`money` DOUBLE NULL DEFAULT 0,
	`gold` DOUBLE NULL DEFAULT 0,
	PRIMARY KEY (`name`)
)
COLLATE='utf8mb4_general_ci'
ENGINE=InnoDB
;


CREATE TABLE `bank_users` (
	`name` VARCHAR(50) NOT NULL COLLATE 'utf8mb4_general_ci',
	`identifier` VARCHAR(50) NOT NULL COLLATE 'utf8mb4_general_ci',
	`money` DOUBLE(22,0) NULL DEFAULT '0',
	`gold` DOUBLE(22,0) NULL DEFAULT '0',
	INDEX `name` (`name`) USING BTREE,
	CONSTRAINT `bank` FOREIGN KEY (`name`) REFERENCES `vorp`.`banks` (`name`) ON UPDATE RESTRICT ON DELETE CASCADE
)
COLLATE='utf8mb4_general_ci'
ENGINE=InnoDB
;

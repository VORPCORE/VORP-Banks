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
  `charidentifier` INT(11) NOT NULL,
	`money` DOUBLE(22,0) NULL DEFAULT '0',
	`gold` DOUBLE(22,0) NULL DEFAULT '0',
	INDEX `name` (`name`) USING BTREE,
	CONSTRAINT `bank` FOREIGN KEY (`name`) REFERENCES `vorp`.`banks` (`name`) ON UPDATE CASCADE ON DELETE CASCADE
)
COLLATE='utf8mb4_general_ci'
ENGINE=InnoDB
;

CREATE TABLE `transactions` (
	`bank` varchar(50) DEFAULT NULL,
  `fromIdentifier` varchar(50) DEFAULT NULL,
  `fromcharidentifier` INT(11) NOT NULL,
  `toIdentifier` varchar(50) DEFAULT NULL,
  `tocharidentifier` INT(11) NOT NULL,
  `date` date DEFAULT NULL,
  `money` double(22,2) DEFAULT 0.00,
  `gold` double(22,2) DEFAULT 0.00,
  `reason` varchar(100) DEFAULT NULL,
  `bankto` varchar(50) DEFAULT NULL,
  KEY `FK_transactions_banks` (`bank`),
  KEY `FK_transactions_banks_2` (`bankto`),
  CONSTRAINT `FK_transactions_banks` FOREIGN KEY (`bank`) REFERENCES `banks` (`name`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `FK_transactions_banks_2` FOREIGN KEY (`bankto`) REFERENCES `banks` (`name`) ON DELETE CASCADE ON UPDATE CASCADE
)
COLLATE='utf8mb4_general_ci'
ENGINE=InnoDB
;



import sys
import os
import subprocess

def get_git_path():
    try:
        res = subprocess.run(["git", "rev-parse", "--git-path", "sdd"], capture_output=True, text=True, encoding='utf-8', check=True)
        return res.stdout.strip()
    except Exception:
        return ".git/sdd"

def extract_task_brief(plan_path, task_num, out_path=None):
    if not os.path.exists(plan_path):
        print(f"no such plan file: {plan_path}", file=sys.stderr)
        sys.exit(2)
        
    if not out_path:
        git_sdd = get_git_path()
        os.makedirs(git_sdd, exist_ok=True)
        out_path = os.path.join(git_sdd, f"task-{task_num}-brief.md")
        
    with open(plan_path, 'r', encoding='utf-8') as f:
        lines = f.readlines()
        
    infence = False
    intask = False
    brief_lines = []
    
    # Match "# Task N" or "### Task N"
    task_header_prefix = f"Task {task_num}"
    
    for line in lines:
        if line.startswith("```"):
            infence = not infence
            
        if not infence:
            # Check if this line is a task header
            stripped = line.strip()
            if stripped.startswith("#") and "Task" in stripped:
                parts = [p.strip() for p in stripped.split() if p.strip()]
                # Check if this is the target task
                # e.g., "###", "Task", "1:", "[Component]"
                is_target = False
                for i in range(len(parts) - 1):
                    if parts[i] == "Task" and parts[i+1].startswith(str(task_num)):
                        # Ensure it's not a different number like 10 when we want 1
                        num_part = "".join(filter(str.isdigit, parts[i+1]))
                        if num_part == str(task_num):
                            is_target = True
                            break
                if is_target:
                    intask = True
                else:
                    # If we hit another task header, stop extracting
                    intask = False
                    
        if intask:
            brief_lines.append(line)
            
    if not brief_lines:
        print(f"task {task_num} not found in {plan_path}", file=sys.stderr)
        sys.exit(3)
        
    os.makedirs(os.path.dirname(out_path), exist_ok=True)
    with open(out_path, 'w', encoding='utf-8') as f:
        f.writelines(brief_lines)
        
    print(f"wrote {out_path}: {len(brief_lines)} lines")
    return out_path

def create_review_package(base, head, out_path=None):
    # Verify base and head
    try:
        subprocess.run(["git", "rev-parse", "--verify", "--quiet", base], capture_output=True, check=True)
        subprocess.run(["git", "rev-parse", "--verify", "--quiet", head], capture_output=True, check=True)
    except Exception as e:
        print(f"Invalid BASE {base} or HEAD {head}", file=sys.stderr)
        sys.exit(2)
        
    if not out_path:
        git_sdd = get_git_path()
        os.makedirs(git_sdd, exist_ok=True)
        # short shas
        base_short = subprocess.run(["git", "rev-parse", "--short", base], capture_output=True, text=True, encoding='utf-8', check=True).stdout.strip()
        head_short = subprocess.run(["git", "rev-parse", "--short", head], capture_output=True, text=True, encoding='utf-8', check=True).stdout.strip()
        out_path = os.path.join(git_sdd, f"review-{base_short}..{head_short}.diff")
        
    log_res = subprocess.run(["git", "log", "--oneline", f"{base}..{head}"], capture_output=True, text=True, encoding='utf-8', errors='replace')
    stat_res = subprocess.run(["git", "diff", "--stat", f"{base}..{head}"], capture_output=True, text=True, encoding='utf-8', errors='replace')
    diff_res = subprocess.run(["git", "diff", "-U10", f"{base}..{head}"], capture_output=True, text=True, encoding='utf-8', errors='replace')
    
    with open(out_path, 'w', encoding='utf-8') as f:
        f.write(f"# Review package: {base}..{head}\n\n")
        f.write("## Commits\n")
        f.write(log_res.stdout or "")
        f.write("\n\n## Files changed\n")
        f.write(stat_res.stdout or "")
        f.write("\n\n## Diff\n")
        f.write(diff_res.stdout or "")
        
    commits_count = len(log_res.stdout.strip().split('\n')) if log_res.stdout and log_res.stdout.strip() else 0
    print(f"wrote {out_path}: {commits_count} commit(s)")
    return out_path

if __name__ == "__main__":
    if len(sys.argv) < 3:
        print("Usage:\n  py sdd_helper.py task-brief PLAN_FILE TASK_NUMBER [OUTFILE]\n  py sdd_helper.py review-package BASE HEAD [OUTFILE]")
        sys.exit(1)
        
    cmd = sys.argv[1]
    if cmd == "task-brief":
        plan = sys.argv[2]
        num = int(sys.argv[3])
        out = sys.argv[4] if len(sys.argv) > 4 else None
        extract_task_brief(plan, num, out)
    elif cmd == "review-package":
        base = sys.argv[2]
        head = sys.argv[3]
        out = sys.argv[4] if len(sys.argv) > 4 else None
        create_review_package(base, head, out)
    else:
        print(f"unknown command {cmd}")
        sys.exit(1)
